namespace InfoSysCrawler

open FSharp.Json
open System.IO
open System.Net.Http
open System.Text.RegularExpressions

open SiteMap

module Request =

    let client = new HttpClient()
    // Fetch the contents of a web page
    // Source: https://dev.to/tunaxor/making-http-requests-in-f-1n0b
    let getAsync (url: Url) =
        task {
            let (Url _url) = url
            return! client.GetStringAsync(_url)
        }
        |> Async.AwaitTask
        |> Async.RunSynchronously

    let matchGroups text pattern =
        Regex.Matches(text, pattern)
        |> Seq.map (fun m -> m.Groups |> Seq.toList)

    // Source: https://stackoverflow.com/a/40399128/6329629
    let count (haystack: string) (needle: string) =
        match needle with
        | "" -> 0
        | _ ->
            (haystack.Length
             - haystack.Replace(needle, "").Length)
            / needle.Length

    let getDepth html = count html "baumIMG" - 1

    let toValidMenuUrl (menuUrl: string) =
        Url(
            "https://infosys.beckhoff.com/english/menu/"
            + menuUrl
        )

    let findFolders (html: string) =
        let folderPattern =
            "<a name.*=\"(menu.php\?id=.*)\"><img.*\">(.*)<\/a>.*<br \/>"

        matchGroups html folderPattern
        |> Seq.map (fun g ->
            let id =
                { Url = (g.[1].Value |> toValidMenuUrl)
                  Name = g.[2].Value }

            g.[0].Value |> getDepth, Folder(Id = id, Nodes = []))
        |> Seq.toList

    let findNodeOfDepth (f: string -> list<int * Node>) (depth: int) (html: string) =
        html
        |> f
        |> List.filter (fun (d, _) -> d = depth)
        |> List.map (fun (_, node) -> node)

    let findFoldersOfDepth = findNodeOfDepth findFolders

    let filterIgnoredPages ignoredPages (nodes: Node list) =
        nodes
        |> List.map (fun node ->
            let id = node |> getId

            if (ignoredPages |> List.contains id.Name) then
                IgnoredPage id
            else
                Page(id, None))

    let toValidUrl (url: string) =
        if url.StartsWith("http") then
            Url(url)
        else
            let infoSysUrlPrefix = "https://infosys.beckhoff.com/"

            Url(infoSysUrlPrefix + url.Substring(3))

    let findPages (html: string) =
        let pagePattern =
            "<a name.*href=\"(.*)\" target=\"main\">.*>(.*)<\/a><\/span><br \/>"

        matchGroups html pagePattern
        |> Seq.map (fun g ->
            let id =
                { Url = (g.[1].Value |> toValidUrl)
                  Name = g.[2].Value }

            g.[0].Value |> getDepth, Page(Id = id, TwinCatVersion = None))
        |> Seq.toList

    let findPagesOfDepth = findNodeOfDepth findPages

    let tryFindTwinCATVersion (html: string) =
        let m =
            Regex.Match(
                html,
                "twincat v(?:ersion)?\s{0,2}(3).(\d)\.?(?:\s{1,2}(?:build|b|>=|>)\s{0,2})?(?<major>\d{1,4})?\.?(?<minor>\d{0,2})?",
                RegexOptions.IgnoreCase
            )

        let notEmptyString (str: string) = str.Length > 0

        match m.Success with
        | true ->
            m.Groups
            |> Seq.tail
            |> Seq.map (fun g -> g.Value)
            |> Seq.filter notEmptyString
            |> String.concat "."
            |> Some
        | false -> None

    let tryGetTwinCatVersion = getAsync >> tryFindTwinCATVersion

    let getLevel url =
        let html = getAsync url
        let (Url u) = url
        let pageId = u.Split "="

        let m =
            Regex.Match(html, $"<a.*{pageId.[1]}.*<br \/>")

        match m.Success with
        | true -> m.Groups.[0].Value |> getDepth
        | false -> 0

    let filterIgnoredFolders ignoredFolders (nodes: Node list) =
        nodes
        |> List.map (fun node ->
            let id = node |> getId

            if (ignoredFolders |> List.contains id.Name) then
                IgnoredFolder id
            else
                Folder(id, []))

    let traverseMenu ignoreFolders ignorePages (start: Url) =
        let rec traverse depth (url: Url) : list<Node> =
            let menuPhp = getAsync url

            let folders =
                menuPhp
                |> findFoldersOfDepth depth
                |> filterIgnoredFolders ignoreFolders

            let pages =
                menuPhp
                |> findPagesOfDepth depth
                |> filterIgnoredPages ignorePages

            seq {
                for node in folders @ pages do
                    match node with
                    | Folder (id, _) ->
                        if depth <= 5 then
                            printfn "%s %s" (String.replicate depth " ") id.Name

                        yield Folder(Id = id, Nodes = traverse (depth + 1) id.Url)
                    | Page (id, _) ->
                        yield
                            Page(Id = id, TwinCatVersion = tryGetTwinCatVersion id.Url)
                    | IgnoredFolder id -> yield IgnoredFolder id
                    | IgnoredPage id -> yield IgnoredPage id
            }
            |> Seq.toList

        let initialDepth = getLevel start + 1
        traverse initialDepth start

    let saveAsJson (fname: string) nodes =
        let json = Json.serialize nodes
        File.WriteAllText(fname, json)

