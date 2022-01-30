# InfoSysCrawler

Crawler for [Beckhoff's InfoSys](https://infosys.beckhoff.com/index_en.htm).

## How it works

The crawler goes through the InfoSys menu and finds all subfolders and pages. It is not necessary to start at a top folder, but you can also start at a lower folder. Optionally some folders/pages can be ignored.

## Usage

This crawler was used to walk through InfoSys pages and look for a TwinCAT version number. The version number is often found on the bottom of a page, as shown below. 

![](img/tc_version.png)

The version numbers are used for the [unofficial TwinCAT changelog](https://github.com/Roald87/TwinCatChangelog).

## Current dumps

In `data/` you will find a few folders which are crawled and saved as a json file. Currently the following folders were crawled where the folders "Foreword", "Installation" and "Samples" and the pages "Overview" and "Search" are ignored. On ignored pages/folders the crawler doesn't try to find a TwinCAT version number. Data was obtained on 30 January 2022.

- `data/plc_libraries.json`: TwinCAT 3/TE1000 XAE/PLC/PLC Libraries
- `data/texxxx.json`: TwinCAT 3/TExxxx | TwinCAT 3 Engineering 
- `data/tc1xxx.json`: TwinCAT 3/TC1xxx | TwinCAT 3 Base
- `data/tfxxxx.json`: TwinCAT 3/TFxxxx | TwinCAT 3 Functions
