module Tests

open FSharp.Json
open FsUnit.Xunit
open System.IO
open Xunit

open InfoSysCrawler.SiteMap
open InfoSysCrawler.Request
open InfoSysCrawler.Analyses

[<Fact>]
let ``Find a single folder`` () =
    let expectedUrl =
        "https://infosys.beckhoff.com/english/menu/menu.php?id=444084617613099609&amp;noHl=1&amp;ancRel=1"

    findFolders
        """<a name="a444084617613099609"></a><img src="../../images/tree/VL.gif" width="8" height="16" alt="" class="baumIMG" /><a href="menu.php?id=444084617613099609&amp;noHl=1&amp;ancRel=1"><img src="../../images/tree/OZ_new.gif" width="14" height="14" alt="" class="baumIMG" /></a> <span class="linkText"><a onclick="javascript:self.location.href='menu.php?id=444084617613099609&amp;ancRel=1';change_color();" href="../../content/1033/ipcinfosys/index.html?id=444084617613099609" target="main" class="baum"  id="e444084617613099609">Industrial PC</a></span><br />"""
    |> should
        equal
        [ (1,
           Folder(
               { Name = "Industrial PC"
                 Url = Url(expectedUrl) },
               Nodes = []
           )) ]

[<Fact>]
let ``Find multiple folders`` () =
    let expectedUrls =
        [ "https://infosys.beckhoff.com/english/menu/menu.php?id=4211986674403809096&amp;noHl=1&amp;ancRel=1"
          "https://infosys.beckhoff.com/english/menu/menu.php?id=4062827605076109961&amp;noHl=1&amp;ancRel=1" ]

    findFolders
        """<a name="a4211986674403809096"></a><img src="../../images/tree/VL.gif" width="8" height="16" alt="" class="baumIMG" /><a href="menu.php?id=4211986674403809096&amp;noHl=1&amp;ancRel=1"><img src="../../images/tree/OZ_new.gif" width="14" height="14" alt="" class="baumIMG" /></a> <span class="linkText"><a onclick="javascript:self.location.href='menu.php?id=4211986674403809096&amp;ancRel=1';change_color();" href="../../content/1033/fieldbusinfosys/index.html?id=4211986674403809096" target="main" class="baum"  id="e4211986674403809096">Fieldbus Components</a></span><br />
        <a name="a4062827605076109961"></a><img src="../../images/tree/VL.gif" width="8" height="16" alt="" class="baumIMG" /><a href="menu.php?id=4062827605076109961&amp;noHl=1&amp;ancRel=1"><img src="../../images/tree/OZ_new.gif" width="14" height="14" alt="" class="baumIMG" /></a> <span class="linkText"><a onclick="javascript:self.location.href='menu.php?id=4062827605076109961&amp;ancRel=1';change_color();" href="../../content/1033/driveinfosys/index.html?id=4062827605076109961" target="main" class="baum"  id="e4062827605076109961">Drive Technology</a></span><br />"""
    |> should
        equal
        [ (1,
           Folder(
               { Name = "Fieldbus Components"
                 Url = Url(expectedUrls.[0]) },
               Nodes = []
           ))
          (1,
           Folder(
               { Name = "Drive Technology"
                 Url = Url(expectedUrls.[1]) },
               Nodes = []
           )) ]

[<Fact>]
let ``Find folders with special characters`` () =
    let folderNames =
        findFolders
            """<a name="a7161397842851083875"></a><img src="../../images/tree/VL.gif" width="8" height="16" alt="" class="baumIMG" /><a href="menu.php?id=7161397842851083875&amp;noHl=1&amp;ancRel=1"><img src="../../images/tree/OZ_new.gif" width="14" height="14" alt="" class="baumIMG" /></a> <span class="linkText"><a onclick="javascript:self.location.href='menu.php?id=7161397842851083875&amp;ancRel=1';change_color();" href="../../content/1033/cxinfosys/index.html?id=7161397842851083875" target="main" class="baum"  id="e7161397842851083875">Embedded-PC</a></span><br />
            <a name="a2683277694279723185"></a><img src="../../images/tree/VL.gif" width="8" height="16" alt="" class="baumIMG" /><a href="menu.php?id=2683277694279723185&amp;noHl=1&amp;ancRel=1"><img src="../../images/tree/OZ_new.gif" width="14" height="14" alt="" class="baumIMG" /></a> <span class="linkText"><a onclick="javascript:self.location.href='menu.php?id=2683277694279723185&amp;ancRel=1';change_color();" href="../../content/1033/tcinfosys3/index.html?id=2683277694279723185" target="main" class="baum"  id="e2683277694279723185">TwinCAT 3</a></span><br />
            <a name="a6455946414529289984"></a><img src="../../images/tree/VL.gif" width="19" height="16" alt="" class="baumIMG" /><img src="../../images/tree/VL.gif" width="8" height="16" alt="" class="baumIMG" /><a href="menu.php?id=6455946414529289984&amp;noHl=1&amp;ancRel=1"><img src="../../images/tree/OZ_new.gif" width="14" height="14" alt="" class="baumIMG" /></a> <span class="linkText"><a onclick="javascript:self.location.href='menu.php?id=6455946414529289984&amp;ancRel=1';change_color();" href="../../content/1033/tcinfosys3/63050399110555147.html?id=6455946414529289984" target="main" class="baum"  id="e6455946414529289984">TExxxx | TwinCAT 3 Engineering</a></span><br />
            <a name="a1871159972225603560"></a><img src="../../images/tree/VL.gif" width="19" height="16" alt="" class="baumIMG" /><img src="../../images/tree/VL.gif" width="19" height="16" alt="" class="baumIMG" /><img src="../../images/tree/VL.gif" width="8" height="16" alt="" class="baumIMG" /><a href="menu.php?id=1871159972225603560&amp;noHl=1&amp;ancRel=1"><img src="../../images/tree/OZ_new.gif" width="14" height="14" alt="" class="baumIMG" /></a> <span class="linkText"><a onclick="javascript:self.location.href='menu.php?id=1871159972225603560&amp;ancRel=1';change_color();" href="../../content/1033/tc3_matlab_overview/index.html?id=1871159972225603560" target="main" class="baum"  id="e1871159972225603560">MATLAB®/Simulink®</a></span><br />"""
        |> List.map (fun (_, node) -> getId node)
        |> List.map (fun id -> id.Name)

    folderNames
    |> should
        equal
        [ "Embedded-PC"
          "TwinCAT 3"
          "TExxxx | TwinCAT 3 Engineering"
          "MATLAB®/Simulink®" ]

[<Fact>]
let ``Return empty list if no folder is found`` () =
    findFolders ""
    |> should equal ([]: list<int * Node>)

[<Fact>]
let ``Find sub folder`` () =
    let expectedUrl =
        "https://infosys.beckhoff.com/english/menu/menu.php?id=3260280473517005700&amp;noHl=1&amp;ancRel=1"

    findFolders
        """<a name="a4446269619822350950"></a><img src="../../images/tree/VL.gif" width="19" height="16" alt="" class="baumIMG" /><img src="../../images/tree/VL.gif" width="19" height="16" alt="" class="baumIMG" /><img src="../../images/tree/VL.gif" width="8" height="16" alt="" class="baumIMG" /><a href="menu.php?id=3260280473517005700&amp;noHl=1&amp;ancRel=1"><img src="../../images/tree/OZ_open_red.gif" width="14" height="14" alt="" class="baumIMG" /></a> <span class="linkText"><a onclick="javascript:change_color();setHighlight('e4446269619822350950');" href="../../content/1033/tc3_overview/190749195.html?id=4446269619822350950" target="main" class="baum"  id="e4446269619822350950">Foreword</a></span><br />"""
    |> should
        equal
        [ (3,
           Folder(
               { Name = "Foreword"
                 Url = Url(expectedUrl) },
               Nodes = []
           )) ]

[<Fact>]
let ``Generate valid menu url`` () =
    toValidMenuUrl "menu.php?id=4062827605076109961&amp;noHl=1&amp;ancRel=1"
    |> should
        equal
        (Url
            "https://infosys.beckhoff.com/english/menu/menu.php?id=4062827605076109961&amp;noHl=1&amp;ancRel=1")

[<Fact>]
let ``Find a page`` () =
    let expectedUrl =
        "https://infosys.beckhoff.com/search/default.aspx?id=8368617021628159453&lg=en"

    findPages
        """<a name="a8368617021628159453"></a><img src="../../images/tree/VL.gif" width="8" height="16" alt="" class="baumIMG" /><a onclick="javascript:change_color();" href="https://infosys.beckhoff.com/search/default.aspx?id=8368617021628159453&lg=en" target="main"><img src="../../images/tree/SE_new.gif" width="14" height="14" alt="" class="baumIMG" /></a> <span class="linkText"><a onclick="javascript:change_color();setHighlight('e8368617021628159453');" href="https://infosys.beckhoff.com/search/default.aspx?id=8368617021628159453&lg=en" target="main" class="baum"  id="e8368617021628159453">Search</a></span><br />"""
    |> should
        equal
        [ (1,
           Page(
               { Url = Url(expectedUrl)
                 Name = "Search" },
               TwinCatVersion = None
           )) ]

[<Fact>]
let ``Make partial url into complete InfoSys url`` () =
    toValidUrl "../../content/1033/rss/bkinfosys_news.htm?id=1027518492792252370"
    |> should
        equal
        (Url
            "https://infosys.beckhoff.com/../content/1033/rss/bkinfosys_news.htm?id=1027518492792252370")

[<Fact>]
let ``Do not change valid InfoSys url`` () =
    toValidUrl
        "https://infosys.beckhoff.com/search/default.aspx?id=8368617021628159453&lg=en"
    |> should
        equal
        (Url
            "https://infosys.beckhoff.com/search/default.aspx?id=8368617021628159453&lg=en")

[<Fact>]
let ``Find multiple pages`` () =
    let expectedUrls =
        [ "https://infosys.beckhoff.com/../content/1033/rss/bkinfosys_news.htm?id=1027518492792252370"
          "https://infosys.beckhoff.com/../content/1033/html/bkinfosys_intro.htm?id=5555681668111771673" ]

    findPages
        """<a name="a1027518492792252370"></a><img src="../../images/tree/VL.gif" width="8" height="16" alt="" class="baumIMG" /><a onclick="javascript:change_color();" href="../../content/1033/rss/bkinfosys_news.htm?id=1027518492792252370" target="main"><img src="../../images/tree/SE_new.gif" width="14" height="14" alt="" class="baumIMG" /></a> <span class="linkText"><a onclick="javascript:change_color();setHighlight('e1027518492792252370');" href="../../content/1033/rss/bkinfosys_news.htm?id=1027518492792252370" target="main" class="baum"  id="e1027518492792252370">News</a></span><br />
        <a name="a5555681668111771673"></a><img src="../../images/tree/VL.gif" width="8" height="16" alt="" class="baumIMG" /><a onclick="javascript:change_color();" href="../../content/1033/html/bkinfosys_intro.htm?id=5555681668111771673" target="main"><img src="../../images/tree/SE_new.gif" width="14" height="14" alt="" class="baumIMG" /></a> <span class="linkText"><a onclick="javascript:change_color();setHighlight('e5555681668111771673');" href="../../content/1033/html/bkinfosys_intro.htm?id=5555681668111771673" target="main" class="baum"  id="e5555681668111771673">Home</a></span><br />"""
    |> should
        equal
        [ (1,
           Page(
               { Url = Url(expectedUrls.[0])
                 Name = "News" },
               TwinCatVersion = None
           ))
          (1,
           Page(
               { Url = Url(expectedUrls.[1])
                 Name = "Home" },
               TwinCatVersion = None
           )) ]

[<Fact>]
let ``Return empty list if no page is found`` () =
    findPages ""
    |> should equal ([]: list<int * Node>)

[<Theory>]
[<InlineData("assets/3_1_0.html")>]
[<InlineData("assets/3_1_4024_7.html")>]
[<InlineData("assets/3_1_4024_11.html")>]
[<InlineData("assets/3_1_4026_0.html")>]
let ``With TwinCAT version on page`` (fname) =
    let html =
        File.ReadAllLines("../../../" + fname)
        |> String.concat ""

    let tcversion =
        Path
            .GetFileNameWithoutExtension(fname)
            .Replace('_', '.')

    tryFindTwinCATVersion html
    |> should equal (Some tcversion)

[<Theory>]
[<InlineData("TwinCAT v3.1 Build 4022.30", "3.1.4022.30")>]
[<InlineData("TwinCAT v3.1. >= 4022.31", "3.1.4022.31")>]
[<InlineData("TwinCAT v3.1. > 4024.35", "3.1.4024.35")>]
let ``TwinCAT version with additional characters/text``
    (
        versionString,
        expectedVersion
    ) =

    tryFindTwinCATVersion versionString
    |> should equal (Some expectedVersion)

[<Theory>]
[<InlineData("assets/no_tc_version.html")>]
[<InlineData("assets/no_tc_version_with_library_version.html")>]
let ``No TwinCAT version on page`` (fname) =
    let html =
        File.ReadAllLines("../../../" + fname)
        |> String.concat ""

    tryFindTwinCATVersion html |> should equal None

[<Theory>]
[<InlineData(4,
             """<a name="a1852163624443967212"></a><img src="../../images/tree/VL.gif" width="19" height="16" alt="" class="baumIMG" /><img src="../../images/tree/VL.gif" width="19" height="16" alt="" class="baumIMG" /><img src="../../images/tree/VL.gif" width="19" height="16" alt="" class="baumIMG" /><img src="../../images/tree/VL.gif" width="8" height="16" alt="" class="baumIMG" /><a href="menu.php?id=1852163624443967212&amp;noHl=1&amp;ancRel=1"><img src="../../images/tree/OZ_new.gif" width="14" height="14" alt="" class="baumIMG" /></a> <span class="linkText"><a onclick="javascript:self.location.href='menu.php?id=1852163624443967212&amp;ancRel=1';change_color();" href="../../content/1033/tf50x0_tc3_nc_ptp/index.html?id=1852163624443967212" target="main" class="baum"  id="e1852163624443967212">TF50x0 | NC PTP</a></span><br />""")>]
[<InlineData(5,
             """<a name="a3563403872127141228"></a><img src="../../images/tree/VL.gif" width="19" height="16" alt="" class="baumIMG" /><img src="../../images/tree/VL.gif" width="19" height="16" alt="" class="baumIMG" /><img src="../../images/tree/VL.gif" width="19" height="16" alt="" class="baumIMG" /><img src="../../images/tree/VL.gif" width="19" height="16" alt="" class="baumIMG" /><img src="../../images/tree/VL.gif" width="8" height="16" alt="" class="baumIMG" /><a onclick="javascript:change_color();" href="../../content/1033/tf5110-tf5113_tc3_kinematic_transformation/1955299083.html?id=3563403872127141228" target="main"><img src="../../images/tree/SE_new.gif" width="14" height="14" alt="" class="baumIMG" /></a> <span class="linkText"><a onclick="javascript:change_color();setHighlight('e3563403872127141228');" href="../../content/1033/tf5110-tf5113_tc3_kinematic_transformation/1955299083.html?id=3563403872127141228" target="main" class="baum"  id="e3563403872127141228">Installation</a></span><br />""")>]
let ``Check depth of a folder and a page`` (expected, html) =
    getDepth html |> should equal expected

[<Trait("Category", "Online")>]
[<Fact>]
let ``crawl real menu`` () =
    let expected =
        Json.deserialize<Node list> (
            File.ReadAllText("../../../assets/expected_traverse_menu.json")
        )

    let url =
        (Url "https://infosys.beckhoff.com/english/menu/menu.php?id=8671909437208972109")

    traverseMenu [ "Foreword" ] [ "Overview" ] url
    |> should equal expected

[<Fact>]
let ``crawl menu ignore foreword folder`` () =
    let expected =
        Json.deserialize<Node list> (
            File.ReadAllText("../../../assets/expected_ignore_folders.json")
        )

    let actual =
        File.ReadAllText("../../../assets/input_ignore_folders_pages.html")
        |> findFoldersOfDepth 6
        |> filterIgnoredFolders [ "Foreword" ]

    actual |> should equal expected


[<Fact>]
let ``crawl menu ignore Overview page`` () =
    let expected =
        [ IgnoredPage(
              Id =
                  { Name = "Overview"
                    Url =
                      Url
                          "https://infosys.beckhoff.com/../content/1033/tcplclib_tc2_coupler/42582795.html?id=7734414512993082394" }
          ) ]

    let actual =
        File.ReadAllText("../../../assets/input_ignore_folders_pages.html")
        |> findPagesOfDepth 6
        |> filterIgnoredPages [ "Overview" ]

    actual |> should equal expected

[<Fact>]
let ``convert page to markdown link`` () =
    let expected =
        "[`Tc3_IPCDiag.FB_IPCDiag_Register`](https://infosys.beckhoff.com/../content/1033/tcplclib_tc3_ipcdiag/1475282443.html?id=3808476523660178266)"

    let p =
        { Name = "FB_IPCDiag_Register"
          Url =
            (Url
                "https://infosys.beckhoff.com/../content/1033/tcplclib_tc3_ipcdiag/1475282443.html?id=3808476523660178266")
          TwinCatVersion = "3.1.4024.7" }

    let actual = toMarkDownLink p

    actual |> should equal expected
