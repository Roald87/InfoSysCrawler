namespace InfoSysCrawler

open System.Text.RegularExpressions

open SiteMap

module Analyses =

    // Collect all pages from a nested Node list
    let getPages (menu: Node list) =
        let rec accumulate nodes pages =
            match nodes with
            | head :: tail ->
                match head with
                | Page (id, tc) ->
                    accumulate tail (Page(Id = id, TwinCatVersion = tc) :: pages)
                | Folder (_, nodes) -> accumulate (nodes @ tail) pages
                | IgnoredPage (_) -> accumulate tail pages
                | IgnoredFolder (_) -> accumulate tail pages
            | [] -> pages

        accumulate menu []

    type Page =
        { Name: string
          Url: Url
          TwinCatVersion: string }

    let groupByVersionNumber pages =
        pages
        |> List.filter (fun page ->
            match page with
            | Page (_, tc) -> tc.IsSome
            | _ -> false)
        |> List.map (fun page ->
            match page with
            | Page (id, tc) ->
                { Name = id.Name
                  Url = id.Url
                  TwinCatVersion = tc.Value }
            | _ ->
                { Name = ""
                  Url = (Url "")
                  TwinCatVersion = "" })
        |> List.groupBy (fun page -> page.TwinCatVersion)
        
    let getLibraryName (url: Url) =
        let (Url u) = url
        let m = Regex.Match(u, @"tcplclib_(.*)/")
    
        let libraryNames = [
            "Tc2_Coupler"
            "Tc2_DALI"
            "Tc2_DataExchange"
            "Tc2_DMX"
            "Tc2_EIB"
            "Tc2_EnOcean"
            "Tc2_EtherCAT"
            "Tc2_GENIbus"
            "Tc2_IoFunctions"
            "Tc2_LON"
            "Tc2_Math"
            "Tc2_MBus"
            "Tc2_MDP"
            "Tc3_MPBus"
            "Tc2_SMI"
            "Tc2_Standard"
            "Tc2_SUPS"
            "Tc2_SystemCX"
            "Tc2_System"
            "Tc2_SystemC69xx"
            "Tc2_Utilities"
            "Tc3_BA_Common"
            "Tc3_DALI"
            "Tc3_DynamicMemory"
            "Tc3_EventLogger"
            "Tc3_IPCDiag"
            "Tc3_JsonXml"
            "Tc3_Module" ]
    
        if m.Success then 
            let libName = m.Groups.[1].Value 
            let index = 
                libraryNames 
                |> List.map (fun x -> x.ToLower()) 
                |> List.findIndex (fun name -> name = libName)
            libraryNames[index]
        else 
            ""
    
    let toMarkDownLink (page: Page) = 
        let (Url u) = page.Url
        $"[`{page.Url |> getLibraryName}.{page.Name}`]({u})"
