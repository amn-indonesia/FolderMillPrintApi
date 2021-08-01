# FolderMillPrintApi
API for receiving file to be printed by FolderMill.

## Server Configuration
### FolderMillPrintApi configuration file (appsettings.json)
All configuration stored in appsettings.json

```javascript
"PrintConfig": {
  "HotFolder": "/data",
  "Printers": [
    "Bullzip PDF Printer",
    "Canon E410 series",
    "EPSON9F450C (L455 Series)",
    "HP ePrint + JetAdvantage"
  ]
}
```

- ```HotFolder``` : This pointing to FolderMill "hot folder". All received file will be stored in this folder to be processed by FolderMill.
- ```Printers``` : List of available printers. This list of current available printer. This information can be used to fill in PrinterName parameter when requesting API to print your file.

### FolderMill Configuration
You should configure FolderMill to watch the same hot folder path as the configuration file.
You should create each workflow for each printer. The file will be processed using filename filter. All files should be ended with printer name information.
This will tell FolderMill what printer that will be used to print the received file.
If the printer name contain space, then the space should be replaced by underscore "_".
For example, if file should be printed with "Canon E410" Printer, then you should create rules Process Only "*Canon_E410.*". Then add action to print filtered document to Canon E410 printer.
Refer to FolderMill documentation page for more information about how to configure FolderMill workflow.

## Request to API
### List of available printer
User can request list of available printer by sending GET request to API URL.
```
GET https://localhost:5500/api/v1/print
```

This will return list of available printer in json array format.

```javascript
["Bullzip PDF Printer", "Canon E410 series", "EPSON9F450C (L455 Series)", "HP ePrint + JetAdvantage"]
```

You can use this information to fullfill PrinterName parameter when invoking API to print a document.


### Request to print a file
This is the json format you should POST to API.

```javascript
{
  "Document": "",
  "FileName": "",
  "PrinterName": "",
  "Username": ""
}
```

- ```Document``` fill this parameter with byte array of file. The byte array should be encoded with Base64.
- ```FileName``` filename you want to print. This necessary to make your job easier when you need to investigate to the HotFolder if FolderMill failed to print the file.
- ```PrinterName``` destination printer. Use value from list of available printer. This will tell FolderMill what printer will be used to print the document.
- ```Username``` name of user who initiate this request.

If your request succeeded, you will get ```HTTP OK 200``` code. But if failed, you will get ```HTTP BADREQUEST 400``` code, with list of description why it failed.


Please note, this Web API project using docker :whale:, so you may want run docker desktop if you already install it, or instll it first before run, or maybe simply choose IISExpress if you want to run it directly.
