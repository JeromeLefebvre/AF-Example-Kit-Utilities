# How to localize an example kit

The current goal is to conver the script into Japansese, thus this documentation and scripts currently only take care of this case.

## Step 0

The kit needs to be of very high quality, or they will be very hard to be localize.

Here is an incomplete things that need to verified.

1. Verify that the all the outputs of analyses that are mapped to tags output a convert method, if not the numbers might turn out very wrong once the conversation to metric occurs
1. Go over each string used to make sure they are clearly written, or else translation might become a great deal tricker if attributes names are essentially variables
1. Ensure there is a description to each object
1. Ensure that configuration attributes are correctly defined as such, if not those values will be erased by a reset to template script
1. To ensure uniqueness of tag creation, each template must be marked with a depth, that states how many parent elements this elements of this templates will have.

## Getting started with running the scrips.

Mark the database and AF server that you will be working with as the default AF database and default AF Server, every script is designed to edit that particular database.

First, a few steps that you will need to do several time.

## Step : Fix Analysis mapping

Whenever you import from a database via xml, all the mappings analysis -> variables are secretly broken. This is fixed by running the `FixAnalaysisMapping.exe` script. Thus, this script must be ran whenever you copy/paste a databse, rename an attribute.

## Step : Reset to database

Many changes to templates are automatically reflected in all elements, many are not. Thus, whenever changes are made to the templates, it is a good idea to run this script. If the changes include big changes to attributes used by the analysis service, a restart of the analysis may be required. The script is: `ResetToTemplate.exe`

## Step : Verify the behaviour of the kit

Change the PI Data Archive element to point to your PI Data Archive, make sure you can create all tags and all analysises are running without errors. No script for this, you simply do this check manually.
If everything works great, you will need to run the "Reset to database" script to continue.

## Step 1 : Import the latest version of the translation table

Import the latest version of the translation tables, this table is a long table that contains all currently verified translations. It also follows the guidelines set by the translation group, for example the word "PI Data Archive" is not actually translated. The table is located in this folder under the name `Translation.xml`.

## Step 2 : Grap all English strings into the translation database

All strings in the database need to be translated, This includes all names, all description, all entries in tables, enumeration sets and anything else you can think off. Thus we add in any new strings not already included in the translation table. The script is `GrabAllStrings.exe`. It prepopluates every entry with a translation from Google Translate.

Now, that all new strings have been added to the database, you now need to translate everything in the database that has not been makred as "Check". Once this is done, please export the strings and email them to Jerome. To export the table, you can run the `ExportTranslationTable.exe` script.

## Step 3 : Create the new tag convention

Japanese characters cannot be included in tag name, thus the need for a system that will translate between Japanese and English. The new tag convention is done by running `ChangeTagNamingConvention.exe`

## Step 4 : Translate the database

Now, we will translate every string that occurs into Japanese. This is done using the `Translate.exe` script.

## Step 5 : Collect all used UOMs and industry research

Import the UOM table and fill it with any new UOMS using `GrabAllUsedUOMs.exe`.
Update all UOMs for your country. All per day rates need to remain per day rates.

## Step 6 : Change currency


## Step 7 : Deal with currency

## Step 8 : Things that need to be answer manually

Manually delete the UOMConversion table

