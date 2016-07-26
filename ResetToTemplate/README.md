# AF Example Kit Utilities - Reset attributes to template

This is a collection of scripts, programs and documentation that can help when dealing designing, consuming and translating [PI AF Example Kits](https://pisquare.osisoft.com/community/all-things-pi/asset-based-pi-example-kits) and more generally any management of AF databases. This particular script is used to reset to template all attibute to their templates and updates the underlying tag if one exists.

## Contents

* `ResetToTemplate.ps1` A PowerShell script that makes use of PI AF SDK to reset to template attributes of a specified database or all databases in a PI System.

## Getting Started

To run the script without argument as in:

`.\ResetToTemplate.ps1`
will refresh the attributes of all elements of all AF databases for the AF Server on localhost. You can specify the AF Server or the AF database as follows:

`.\ResetToTemplate.ps1 -af localhost -database "utilites kit"`

The script will then automatically update all attributes for only that database.

## Requirements

The Powershell script were tested only with the following versions. It may work on prior versions with slight modifications.

* PI System Explorer - Version 2016 SP2 (2.8.2)
* PI AF Server - Version 2016 (2.8)
* Powershell - Version 5.0
* PSA License is required to make use of PI AF SDK

## Development

The script was created using Microsoft [Visual Studio Code](https://code.visualstudio.com/) and the version 0.6.1 of the [PowerShell Extsion](https://marketplace.visualstudio.com/items?itemName=ms-vscode.PowerShell).

## Maintainers

* [Jerome Lefebvre](https://github.com/jeromelefebvre)
* Joon Hyung Im

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

