# PS4_Tools [![Build status](https://ci.appveyor.com/api/projects/status/ftuj48025au3q3cg?svg=true)](https://ci.appveyor.com/project/xXxTheDarkprogramerxXx/ps4-tools)
![](https://camo.githubusercontent.com/565ba7e05b2aa40973cb239eb7ee4bd387488e6b/68747470733a2f2f7878787468656461726b70726f6772616d65727878782e6769746875622e696f2f496d616765732f7073345f746f6f6c732e706e67)

Collection Of Open Source PS4 Tools all in one Library 

Supports Unity 

![](https://img.shields.io/static/v1?label=.net%203.5&message=Old%20School&color=informational&style=for-the-badge)

## Projects Currently Running PS4 Tools

PS4 PS2 Classics Manager (Unreleased) [Github Source](https://github.com/xXxTheDarkprogramerxXx/PS2-Classics-Replacer)

PS4 PS2 Classics GUI [Github Source](https://github.com/xXxTheDarkprogramerxXx/PS3Tools/tree/master/PS4%20PS2%20Classis%20GUI)

PS4 PKG Installer (PKG Store) [Github Source](https://github.com/xXxTheDarkprogramerxXx/Store-Remote-Tool)

PS4 PKG Installer Andorid [Github Source](https://github.com/xXxTheDarkprogramerxXx/Store-Remote-Tool-Android)

PS4 PKG Tool- By pearlxcore [Github Source](https://github.com/pearlxcore/PS4-PKG-Tool)

PS4 Explorer - By Lapy [Github Source](https://github.com/Lapy055/PS4-Xplorer) | [Twitter](https://twitter.com/Lapy05575948)

PS4 Tools Homebrew (1.3) [Github Source](https://github.com/xXxTheDarkprogramerxXx/PS4-Tools-Homebrew) | [Github Release](https://github.com/xXxTheDarkprogramerxXx/PS4_Tools/releases/tag/HB_V1.3)

PS4 Save Manager [Github Source](https://github.com/xXxTheDarkprogramerxXx/PS4_Tools/tree/master/SaveData_Manager)

PS4 Trophy Unlocker [Github Source](https://github.com/xXxTheDarkprogramerxXx/PS4_Tools/tree/master/TrophyUnlocker)

## Project status
![GitHub all releases](https://img.shields.io/github/downloads/xXxTheDarkprogramerxXx/PS4_Tools/total?label=Total%20Downloads&style=for-the-badge)

![GitHub repo size](https://img.shields.io/github/repo-size/xXxTheDarkprogramerxXx/PS4_Tools?style=for-the-badge&label=Repo%20Size)

[![https://github.com/xXxTheDarkprogramerxXx/PS4_Tools/releases/latest](https://img.shields.io/github/v/release/xXxTheDarkprogramerxXx/PS4_Tools?label=Latest%20Release&style=for-the-badge)](https://github.com/xXxTheDarkprogramerxXx/PS4_Tools/releases/latest)


## Example Applications
### Image Util [![Build status](https://ci.appveyor.com/api/projects/status/ftuj48025au3q3cg?svg=true)](https://ci.appveyor.com/project/xXxTheDarkprogramerxXx/ps4-tools)

Created for DefaultDNB

![](https://i.imgur.com/yFVDU7S.png)

### Atrac9 Player

A simple At9 player

![image](https://i.imgur.com/p5EIC5Z.png)

### Ps4 Tools Homebrew
Created originally just to show the power of ps4 tools but now has alot more feutures 

![](https://user-images.githubusercontent.com/12253240/141970667-e7645432-093b-4c8d-8119-e1041725be89.jpg)

### Ps4 PKG Viewer
Created as an example app reads ps4 pkg's
[Github](https://github.com/xXxTheDarkprogramerxXx/PS4_Tools/tree/master/PS4_PKG_Viewer)

![](https://i.imgur.com/VQWOk5Z.png)

### PS4 Save Manager 
Created to manage save files dumped with either Ps4 Tools Homebrew or with Save Data Mounter by Chendo Chap

![](https://i.imgur.com/BV4fpiW.png)

### PS4 Trophy Unlocker
Well It Unlocks Trophies

![image](https://i.imgur.com/ss9B5iO.png)

![image](https://i.imgur.com/5eWg3Q1.png)

### Ps4 Trophy Viewer
Created on the request of @pearlxcore

![](https://i.imgur.com/3B56fVw.png)

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
    │   ├──   ├── ReadAllUnprotectedData          /*Deprecated*/
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

#### Reading a PKG File (Using Official Toolset) /*Deprecated 2018-11-18*/
```c#

```

#### Reading and Saving a GP4
```c#
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
```c#
var item = PS4_Tools.Image.DDS.GetBitmapFromDDS(@"C:\Users\3deEchelon\Desktop\PS4\psp Decrypt\Sc0\icon0.dds");
            pictureBox1.Image = item;
```

#### Dumping a RCO File
```c#
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
     PS4_Tools.PKG.SceneRelated.Unprotected_PKG ps4pkg = PS4_Tools.PKG.SceneRelated.Read_PKG(@"C:\Users\3deEchelon\Desktop\PS4\Euro.FISHING.COLLECTORS.EDITION.PS4-DUPLEX\Euro.Fishing.Collectors.Edition.PS4-DUPLEX\duplex-euro.fishing.collectors.ed.ps4\Euro.Fishing.Collectors.Edition.PS4-DUPLEX.pkg");

            /*Lets work with the data shall we*/
            /*Display the PSFO in some type of info format*/
            var item = ps4pkg.Param;
           
            for (int i = 0; i < item.Tables.Count; i++)
            {
                listBox3.Items.Add(item.Tables[i].Name + ":" + item.Tables[i].Value);
            }
            /*Display Image*/
            pictureBox2.Image = ps4pkg.Image;

            var trphy = ps4pkg.Trophy_File;

            for (int i = 0; i < trphy.FileCount; i++)
            {
                listBox4.Items.Add(trphy.trophyItemList[i].Name);
            }
```

## Credits

* [Maxton](https://github.com/maxton) - For The Amazing Work He has done for the scene ! (LibOrbisPKG)
* [GarnetSunset](https://github.com/GarnetSunset) - Playstation Store DLC Indexer
* [stooged](https://github.com/stooged) - PS4 DLC Indexer (C#)
* [cfwprph](https://github.com/cfwprpht) - His help and Vita Rco extractor tool
* [IDC](https://github.com/idc) - His PS4 Pup Extractor and other work he has done
* [Leecherman](https://sites.google.com/site/theleecherman/) - His tools are always a great reference for me and does some great work
* [Thealexbarney](https://github.com/Thealexbarney) - His great research done on atrac9 files and decoding them 
* [RedEye-32](https://github.com/Red-EyeX32) - His help on getting ESFM decrypted (files inside of trophies)

## Download
[Download](https://github.com/xXxTheDarkprogramerxXx/PS4_Tools/releases/tag/PS4-Tools-(AppVoyer)) - POWER BY APPVOYER

## Open Source Projects
### My Neighborhood 

This is an open source version (alternative) of the PS4 Neighborhood that came with the official ps4 sdk 

Follow development board on 
[Trello](https://trello.com/c/DnFTKOsb/58-ps4-neighborhood-open-source)

![](https://i.imgur.com/h7wZ6Ta.png)

#### What can i do with this ?

This is mainly for developer use on any console that has unsigned code enabled and has the corresponding api installed (release to be announced)

With this you can create your own application / games for ps4 homebrew and instead of creating a pkg each time you can simply attach the application / game to your file serving directory and load your current application in real time (that's the idea anyway)

The project is to resemble the official PlayStation 4 Neighborhood without any of the SCE tools


#### Requirements 

* API Installed
* System must be running a semi dev unit (not normal retail)

To change your console from Normal Retail to a Semi Dev one simply use [LightningMods's Updater](http://psarchive.darksoftware.xyz/UPDATER_BETA.pkg)


Console output will look similar to this 
![](https://i.imgur.com/Lu6z9dv.png)
