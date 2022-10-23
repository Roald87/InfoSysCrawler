namespace InfoSysCrawler

module SiteMap =
    type Url = Url of string
    type Id = { Name: string; Url: Url }

    type Node =
        | Folder of Id: Id * Nodes: list<Node>
        | Page of Id: Id * TwinCatVersion: Option<string>
        | IgnoredFolder of Id: Id
        | IgnoredPage of Id: Id

    let getId node =
        match node with
        | Folder (Id = id) -> id
        | Page (Id = id) -> id
        | IgnoredFolder (Id = id) -> id
        | IgnoredPage (Id = id) -> id
