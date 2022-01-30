# Usage

1. Build the project using `dotnet build`.
2. Open a FSharp interactive console.
3. Import the dll using `#r "InfoSysCrawler/bin/Debug/net6.0/InfoSysCrawler.dll"`, where you should change the path of the dll according to where it is saved on your local system.
4. Then you can crawl a subfolder as follows:
    ```fsharp
    open InfoSysCrawler.SiteMap
    open InfoSysCrawler.Request

    let url = (Url "https://infosys.beckhoff.com/english/menu/menu.php?id=8644252870837316006") // TC1xxx - TwinCAT 3 base // 5 s
    // Add folder names you do not want to into
    let ignoreFolders = ["Foreword"; "Installation"; "Samples"]
    // Add pages which you do not want to look for a twincat version number
    let ignorePages = ["Overview"; "Search"]
    // Start crawling
    let menu = traverseMenu ignoreFolders ignorePages url

    // Output you should see
    TC1000 | TwinCAT 3 ADS
    TC1100 | TwinCAT 3 I/O
    TC1200 | TwinCAT 3 PLC
    TC1210 | TwinCAT 3 PLC/C++
    TC1220 | TwinCAT 3 PLC/C++/MATLAB®/Simulink®
    TC1250 | TwinCAT 3 PLC/NC PTP 10
    TC1260 | TwinCAT 3 PLC/NC PTP 10/NC I
    TC1270 | TwinCAT 3 PLC/NC PTP 10/NC I/CNC
    TC1275 | TwinCAT 3 PLC/NC PTP 10/NC I/CNC E
    TC1300 | TwinCAT 3 C++
    TC1320 | TwinCAT 3 C++/MATLAB®/Simulink®
    ```
5. You can save the crawled data with `menu |> saveAsJson "menu.json"`
6. You can later open the data again with
    ```fsharp
    #r "InfoSysCrawler/FSharp.Json.dll"

    open FSharp.Json

    let deserialized = Json.deserialize<Node list> (File.ReadAllText("menu.json"))
    ```