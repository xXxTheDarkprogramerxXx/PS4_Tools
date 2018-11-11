# PS4_Tools
Collection Of Open Source PS4 Tools all in one Library 

## Getting Started

Add the .DLL to your solution. 
Done Dusted and ready to use

### Class Structure

The class strucutre might change in future releases

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
    |   ├──   ├── Read_PKG                        /*Reads all unprotected data from a pkg (Powered by Leecherman)*/
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
/*Gets a list of unprotected items*/
var lstitem =   PS4_Tools.PKG.Official.ReadAllUnprotectedData(@"C:\Users\3deEchelon\Downloads\Patapon_Remastered_CUSA07184_update_1.01.pkg");
/*Reads a SFO File From an PKG File*/
 Param_SFO.PARAM_SFO sfo =      PS4_Tools.PKG.SceneRelated.PARAM_SFO.Get_Param_SFO(@"C:\Users\3deEchelon\Downloads\Patapon_Remastered_CUSA07184_update_1.01.pkg");
            for (int i = 0; i < sfo.Tables.Count; i++)
            {
                listBox2.Items.Add( sfo.Tables[i].Name + " : " + sfo.Tables[i].Value);
            }
```

#### Reading and Saving a GP4
```
 PS4_Tools.PKG.SceneRelated.GP4.Psproject project =   PS4_Tools.PKG.SceneRelated.GP4.ReadGP4(@"C:\Users\3deEchelon\Documents\Sony\Crash Bandcioot Twinsanity.gp4");
            if(project.Fmt != "gp4")
            {
                MessageBox.Show("This is not a valid PS4 Project");
                return;
            }

            //lets validate some pkg item info before saving
            if(project.Volume.Package.Passcode.Length != 32)
            {
                MessageBox.Show("Passcode Lentgh is not valid");
            }

            //to save a gp4 

            PS4_Tools.PKG.SceneRelated.GP4.SaveGP4(@"C:\Users\3deEchelon\Documents\Sony\tempworking.gp4", project);
```

#### Displaying a dds file 
```
var item = PS4_Tools.Image.DDS.GetBitmapFromDDS(@"C:\Users\3deEchelon\Desktop\PS4\psp Decrypt\Sc0\icon0.dds");
            pictureBox1.Image = item;
```

#### Dumping a RCO File
```
 PS4_Tools.RCO.DumpRco(@"C:\Users\3deEchelon\Desktop\PS4\RCO\Sce.Cdlg.GameCustomData.rco");
```

#### Playing a at9 file 
```c#
 PS4_Tools.Media.Atrac9.LoadAt9(@"C:\Users\3deEchelon\Desktop\PS4\AT9\prelude1.at9");
```

#### Get Game Update Information 
```c#
            var item =PS4_Tools.PKG.Official.CheckForUpdate(textBox1.Text);

            /*TitleID Patch Data Is Avaiavle Here*/

            /*Build some string*/
            string update = label1.Text;
            update += "\n Version : " + item.Tag.Package.Version;
            int ver = Convert.ToInt32(item.Tag.Package.System_ver);
            update += "\n System Version : " + ver.ToString("X");
            update += "\n Remaster : " + item.Tag.Package.Remaster;
            update += "\n Manifest File Number of Pieces : " + item.Tag.Package.Manifest_item.pieces.Count;

            label1.Text = update;
```
#### Get Store Items From a Title ID 
``` c#
    var storeitems = PS4_Tools.PKG.Official.Get_All_Store_Items(textBox1.Text);
```

#### Get Unprotected Data From PKG 
```c#
    
```

## Credits

* [Maxton](https://github.com/maxton) - For The Amazing Work He has done for the scene ! (LibOrbisPKG)
* [GarnetSunset](https://github.com/GarnetSunset) - Playstation Store DLC Indexer
* [stooged](https://github.com/stooged) - PS4 DLC Indexer (C#)
* [cfwprph](https://github.com/cfwprpht) - His help and Vita Rco extractor tool
* [IDC](https://github.com/idc) - His PS4 Pup Extractor and other work he has done
* [Leecherman](https://sites.google.com/site/theleecherman/) - His tools are always a great reference for me and does some great work

