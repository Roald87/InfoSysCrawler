open System
open Argu

open InfoSysCrawler.Request
open InfoSysCrawler.SiteMap

type CliError = | ArgumentsNotSpecified

type Arguments =
    | Path of path: string
    | Json of path: string

    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | Path _ -> "Crawl the InfoSys url recusively"
            | Json _ -> "Json filename to save the crawl results into"

let getExitCode result =
    match result with
    | Ok () -> 0
    | Error err ->
        match err with
        | ArgumentsNotSpecified -> 1

let runCrawler path filename =
    // Add folder names you do not want to into
    let ignoreFolders =
        [ "Foreword"
          "Installation"
          "Samples" ]
    // Add pages which you do not want to look for a twincat version number
    let ignorePages = [ "Overview"; "Search" ]

    let menu =
        traverseMenu ignoreFolders ignorePages (Url path)

    saveAsJson filename menu 

    Ok()


[<EntryPoint>]
let main argv =
    let errorHandler =
        ProcessExiter(
            colorizer =
                function
                | ErrorCode.HelpText -> None
                | _ -> Some ConsoleColor.Red
        )

    let parser =
        ArgumentParser.Create<Arguments>(
            programName = "infosys",
            errorHandler = errorHandler
        )

    match parser.ParseCommandLine argv with
    | p when p.Contains(Path) -> runCrawler (p.GetResult Path) (p.GetResult Json)
    | _ ->
        printfn "%s" (parser.PrintUsage())
        Error ArgumentsNotSpecified
    |> getExitCode
