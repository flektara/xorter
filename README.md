## dev-Xorter##
####About:####
Simple folder organizer. 

Configurable file extension support.

Cleans the selected directory from duplicate files and sorts them according to selected sorting type.

TypeSorter - sorts all files to folders categorised by file type (images, music, etc.)
DateSorter - sorts all files to folders categorised by creation date.

####Configuration:####
Open the app.config file and add a element for folder name and extensions to the xml
 (see RAW readme)
<Images>
  <Extension>.jpg</Extension>
  <Extension>.png</Extension>
</Images>

- all files with extension .jpg,.png are going to be sorted in a folder named 'Images'

Logs are created in the solution directory under FileSorter\FileSorter.UI\bin\Debug\Logs\xorterlogs.txt

####Note:####
Run only against testing data
