# PS4_Tools
Collection Of Open Source PS4 Tools all in one Library 

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on how to deploy the project on a live system.
Add the DLL to your solution. Done Dusted and ready

### Class Structure

The Class Strucutre Might Change in future releases

    namespace PS4_Tools
    ├── PS4_Tools                                 /*Some Defualt Methods For the Tools*/
    │   ├── AppCommonPath()                       /*Returns Working Directory For Tools*/
    │   ├── DeleteDirectory()                     /*Recursive Deletes Directory*/
    ├── SELF                                      /* Reserved class for SELF/ELF Handeling*/
    ├── Media                                     /* PS4 Media Class*/
    │   ├── Atrac9                                /*Atrac9 Class*/
    │   ├──   ├── LoadAt9                         /*Loads Atrac9 Method still in the works*/
    ├── Image                                     /* PS4 Image Class*/
    |   ├── PNG                                   /*PNG Class*/
    │   ├──   ├── Create_PS4_Compatible_PNG       /*Creates a Complatible PS4 PNG*/
    │   ├── DDS                                   /*DDS Class*/
    │   ├──   ├── SavePNGFromDDS                  /*Saves a PNG from a DDS File*/
    │   ├──   ├── GetStreamFromDDS                /*Gets a Stream from a DDS*/   
    │   ├──   ├── GetBitmapFromDDS                /*Gets a Bitmap from a DDS*/
    │   └── GIMImages                             /*GIM Image Class*/
    ├── RCO                                       /* PS4 RCO Class*/
    │   ├── DumpRco                               /*Dumps a Rco File*/
    ├── SaveData                                  /* PS4 SaveData Reserved Class*/
    ├── PKG                                       /* PS4 PKG Handling Class*/ 
    │   ├── Official                              /*Some Methods for Official PKG Items*/
    │   ├──   ├── ReadAllUnprotectedData          /*Retruns All Unprotected Data From a PS4 PKG (NO Passcode Section)*/
    │   ├──   ├── StoreItems                      /*Store Items Object Class (Placeholder)*/
    │   ├──   ├── CheckForUpdate                  /*Returns a Update_Structure Type*/
    │   ├──   ├── Get_All_Store_Items             /*Returns a List<StoreItems> With Download Links and some other infrmation*/
    │   ├── SceneRelated                          /*Some Methods for Scene Related PKG Items*/
    |   ├──   ├── GP4                             /*GP4 File Class*/
    |   ├──   ├──   ├── ReadGP4                   /*Reads a GP4 File Into a Custom Object*/    
    |   ├──   ├──   ├── SaveGP4                   /*Saves a GP4 File From a Custom Object*/
    |   ├──   ├── Create_FKPG                     /*Creates a FPKG (addon file from store items for a spesific item*/
    |   ├──   ├── IDS                             /*IDS Reserved Class*/
    |   ├──   ├── PARAM_SFO                       /*Param.SFO Reserved Class*/
    |   ├──   ├──   ├── Get_Param_SFO             /*Reads a Param SFO into a Param.sfo structure*/
    |   ├──   ├── NP_Data                         /*NP_Data Reserved Class*/
    |   ├──   ├── NP_Title                        /*NP_Title Reserved Class*/
    |   ├──   ├── ReadPKG                         /*Reads a PKG File (Powered by maxtron)*/
    |   ├──   ├── Rename_pkg_To_ContentID         /*Renames a PKG File to the Content ID of the SFO*/    
    |   ├──   ├── Rename_pkg_To_Title             /*Renames a PKG File to the Title of the SFO*/
    │   ├── PS2_Classics                          /*Class For Building PS2 Classics*/
    |   ├──   ├── Create_Single_ISO_PKG           /*Creates a Single ISO File PS2 Classic*/    
    |   ├──   ├── Create_Multi_ISO_PKG            /*Creates a Multie ISO File PS2 Classic*/
    │   ├── PSP_HD                                /*Class For Building PSP HD Items*/
    │   ├── PUP                                   /*Class For PUP Tools*/
    |   ├──   ├── Unpack_PUP                      /*Unpacks a PUP Files*/
    └── (More to come)
    
### Using PS4_Tools

Please see the testers form to see how some of the classes work if not documented here

#### Reading a PKG File 
```
var lstitem = PS4_Tools.PKG.Official.ReadAllUnprotectedData(@"C:\Users\3deEchelon\Downloads\Patapon_Remastered_CUSA07184_update_1.01.pkg");
```
