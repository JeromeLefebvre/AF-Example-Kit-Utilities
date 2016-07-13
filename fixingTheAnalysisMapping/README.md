# AF Example Kit Utilities - Refresh all analysis mapping

This is a collection of scripts, programs and documentation that can help when dealing designing, consuming and translating PI AF Example Kits and more generally any AF Databases. This particular script is used to refresh all analysis mappings, that is it forces a refresh of the information in AF that states which attributes is the output attributes of a particular calculation. This information can get in a bad state if the XML export of an AF Database is imported on a system other than the one that created it.

## Contents

* A PowerShell script that makes use of the PI AF SDK to refresh a specified database or all databases in a PI System.

## Getting Started

To run the script without argument as in:
`.\RefreshMappings.ps1`
Will refresh all mappings of all AF databases for the default AF Server. You can specify the AF Server or the AF Database as follows:
`.\RefreshMappings.ps1 -af localhost -database "utilites kit"`

The script will then automatically update all mappings.

## Requirements

The Powershell script were tested only with the following versions. They may work on prior versions with slight modifications.

* PI System Explorer - Version 2016 SP1 (2.8.1)
* PI AF Server - Version 2016 (2.8)
* Powershell - Version 5.0
* PSA License is required to make use of PI AF SDK

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

