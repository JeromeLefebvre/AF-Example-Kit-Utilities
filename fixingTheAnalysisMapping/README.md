# AF Example Kit Utilities - Refresh analysis mapping

This is a collection of scripts, programs and documentation that can help when dealing designing, consuming and translating [PI AF Example Kits](https://pisquare.osisoft.com/community/all-things-pi/asset-based-pi-example-kits) and more generally any management of AF databases. This particular script is used to refresh all analysis mappings, that is it forces a refresh of the information in AF that states which attributes is the output attributes of a particular analysis. This information can get in a bad state if the XML export of an AF Database is imported on a system other than the one that created it. For example, if an attribute is no longer the output of an analysis after it is renamed, the mapping may need to be refreshed before any renaming occurs.

## Contents

* `RefreshMappings.ps1` A PowerShell script that makes use of PI AF SDK to refresh a specified database or all databases in a PI System.

## Getting Started

To run the script without argument as in:

`.\RefreshMappings.ps1`
will refresh the mappings of all analyses of all AF databases for the default AF Server. You can specify the AF Server or the AF database as follows:

`.\RefreshMappings.ps1 -af localhost -database "utilites kit"`

The script will then automatically update all mappings for only that database.

## Requirements

The Powershell script were tested only with the following versions. It may work on prior versions with slight modifications.

* PI System Explorer - Version 2016 SP1 (2.8.1)
* PI AF Server - Version 2016 (2.8)
* Powershell - Version 5.0
* PSA License is required to make use of PI AF SDK

## Development

The script was created using Microsoft [Visual Studio Code](https://code.visualstudio.com/) and the version 0.6.1 of the [PowerShell Extsion](https://marketplace.visualstudio.com/items?itemName=ms-vscode.PowerShell).

## Maintainers

* [Jerome Lefebvre](https://github.com/jeromelefebvre)


## PI Square

You post feedback of this script on the associated [PI Square Blog post](https://pisquare.osisoft.com/community/all-things-pi/asset-based-pi-example-kits/blog/2016/07/13/localizing-the-af-example-kits-part-1-fixing-the-analysis-mapping-using-afsdk)

## Licensing

Copyright 2016 OSIsoft, LLC.

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.

Please see the file named [LICENSE.md](LICENSE.md).

