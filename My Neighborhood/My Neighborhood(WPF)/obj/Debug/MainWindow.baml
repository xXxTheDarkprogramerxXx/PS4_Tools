   M S B A M L   `   `   `  ����H  DMahApps.Metro, Version=1.6.5.1, Culture=neutral, PublicKeyToken=nullV;clr-namespace:MahApps.Metro.Controls;assembly=MahApps.MetroMahApps.Metro.Controls  5   	    My Neighborhood(WPF)%   My_Neighborhood_WPF_.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35` \RibbonControlsLibrary, Version=4.0.0.11019, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   PControls;clr-namespace:MahApps.Metro.Controls;assembly=MahApps.Metro     5   	   -   5        (   "MahApps.Metro.Controls.MetroWindow   Name$  RibbonWindow��5   	    � Title$G ANeighborhood for PlayStation®4 (Open Source by TheDarkprogramer)��5   
    � UseLayoutRounding .  � Topmost .     ShowTitleBar .    ShowCloseButton .    ShowMaxRestoreButton .    ShowMinButton . $��1299.3��5   	   $	��480��6    � Icon$ Images/Obris.ico��6%   	 � WindowStartupLocation$	 CenterScreen=�6y   &   System.Windows.Media.TextOptions
   TextFormattingMode$
 Ideal=�5      .��5      � -   5         �Name$ 
LayoutRoot��5       � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   +  %System.Windows.Controls.Ribbon.Ribbon  -   5          Name$ Ribbon��5   &   $y�Visible=�6   
���   �
������� �� SelectedIndex$ 1��6�    J� RenderTransformOrigin$ 0.5,0.5&�6�   z�5      V� 5      .~�5      �� �� 5      �� 5      T� 5       T� Y$	 -20Q�6)   5         ApplicationMenu 5,      :  4System.Windows.Controls.Ribbon.RibbonApplicationMenu  5-      5  /System.Windows.Controls.Ribbon.RibbonMenuButton   SmallImageSource$ Images\favicon.ico��6(      Label$
 File��6N   $y�	Collapsed=�6[   .@�5.      >  8System.Windows.Controls.Ribbon.RibbonApplicationMenuItem  -   5          Name$ 	MenuItem1��5/   7   $��Hello _Ribbon��5.   0   3  -System.Windows.Controls.Ribbon.RibbonMenuItem   ImageSource$ Images\Hood1.png��50   7   51      .@�53      .  (System.Windows.Controls.Ribbon.RibbonTab  -   5          Name$ FileTab��53      $
��File��54        55      -   5        $ HomeTab��55      $
��Home��56      .@�57      0	  *System.Windows.Controls.Ribbon.RibbonGroup	  -   5         	 Name$ rgbxApps��57      $
��Apps��58   %   .@�59        -   5          Name$ btnApps��59   '      LargeImageSource$ Images\PS4Logo.png��5:   *   $
 Apps��5;   *   .@�5<        5=      5@      	  5A      -	   5        $ 
rgbxStatus��5A      $��Status��5B   %   .@�5C      1
  +System.Windows.Controls.Ribbon.RibbonButton
  -
   5         
 Name$ 
btnConnect��5C   #    
  SmallImageSource$  Images\icons8-enter-96.png��67    
  Label$ Connect��6e   {�.  6�   
  5D      -   5        $ btnDisconnect��5D   #   $! Images\icons8-export-96.png��6:   $ 
Disconnect��6i   {�.  6�   
  5E      -   5        $ 	btnReboot��5E   #   $! Images\icons8-loader-96.png��66   $ Reboot��6e   {�.  6�   
  5F      -   5        $ 
btnPowerOn��5F   #   $( "Images\icons8-shutdown-128 (1).png��67   $ Power on��6m   $	��Top=�6~   6�   
  5G      -   5        $ btnPowerOff��5G   #   $$ Images\icons8-shutdown-128.png��68   $ 	Power off��6j   {�.  6�   
  5H      -   5        $ btnRestMode��5H   #   $( "Images\icons8-shutdown-128 (2).png��68   $ 	Rest mode��6n   {�.  6�   5I      	  5J      -   5        $ rgbxRun��5J      $	��Run��5K   %   .@�5L        -   5        $ 
btnLoadExe��5L   '   $% Images\icons8-load-cargo-96.png��6;   $ Load executables��6n   {�.  .@�5M        5N      
  5O      -   5        $ 
btnLoadApp��5O   #    
  LargeImageSource$* $Images\icons8-window-settings-96.png��67   $ Load application��6o   {�.  6�     5P      -   5        $ btnKillProcess��5P   '   $# Images\icons8-multiply-52.png��6?   $ Kill process��6p   {�.  .@�5Q        5R        5S      -   5        $ btnCodeDump��5S   '   $$ Images\icons8-processor-96.png��6<   $ Create Core Dump��6n   {�.  .@�5T        5U        5V      -   5        $ 
btnPkgsEnt��5V   '   $ Images\icons8-gift-96.png��6;   $ Packages and entitlments��6h   {�.  .@�5W      
  -   5        $ btnInstallPKG��5W   (   $ Images\icons8-gift-96.png��6?   $ Install PKG��6l   6�   5X      5Y      	  5Z      -   5        $ rgbxTargets��5Z      $��Targets��5[   %   .@�5\      
  -   5        $ btnAddTarget��5\   #   $$ Images\icons8-plus-math-52.png��5]   *   $ 
Add target��5^   *   6=   
  5_      -   5        $ btnFilterTarget��5_   #   $" Images\icons8-filter-100.png��5`   *   $ Filter target��5a   *   6@     5b      -   5        $ btnGroupTarget��5b   '   $ Images\icons8-list-96.png��5c   *   $ Group by��5d   *   .@�5e        5g      
  5h      -   5        $ btnSetDefault��5h   #   $ Images\icons8-star-96.png��5i   *   $ set default target��5j   *   {�.  6W   
  5k      -   5        $ btnRemoveTarget��5k   #   $  Images\icons8-minus-96.png��5l   *   $ Remove target��5m   *   {�. $	��Top=�6Q   6i   
  5n      -   5        $ 
btnRefresh��5n   #   $/ )Images\icons8-update-left-rotation-96.png��5o   *   $ Refresh��5p   *   {�. 6K   5q      	  5r      -   5        $ rgbxData��5r      $
��Data��5s   %   .@�5t      
  -   5        $ 
btnExplore��5t   #   $( "Images\icons8-opened-folder-96.png��5u   *   $ Explore file system��5v   *   {�.  6X   
  5w      -    5        $ btnMapfilesys��5w   #   $( "Images\icons8-shared-folder-96.png��5x   *   $ Map file system��5y   *   6B   
  5z      -!   5        $ btnSetfilserdir��5z   #   $ Images\icons8-list-96.png��5{   *   $  Set file serving direcotry��5|   *   6M   
  5}      -"   5        $ btnCopyfiles��5}   #   $( "Images\icons8-opened-folder-96.png��5~   *   $ Copy file(s)��5   *   {�.  6Q   5�      	  5�      -#   5        $ rgbxPSVR��5�      $��PlayStation VR��5�   %   .@�5�      
  -$   5        $ btnHMDOn��5�   #   $( "Images\icons8-shutdown-128 (1).png��5�   *   $ HMD on��5�   *   {�.  69   
  5�      -%   5        $ 	btnHMDOff��5�   #   $$ Images\icons8-shutdown-128.png��5�   *   $ HMD off��5�   *   {�.  6:   5�      	  5�      -&   5        $ rgbxMORE��5�      $
��More��5�   %   .@�5�      
  -'   5        $ 	btnPlayGo��5�   #   $# Images\icons8-database-96.png��5�   *   $ Play Go��5�   *   {�.  6L   
  5�      -(   5        $ btnTagetSettings��5�   #   $% Images\icons8-wrench-96 (1).png��5�   *   $ Target Settings��5�   *   {�.  6B     5�      -)   5        $ btnMore��5�   '   $% Images\icons8-wrench-96 (1).png��5�   *   $
 More��5�   *   {�. .@�5�        -*   5        $ btnTheme��5�   +   $% Images\icons8-wrench-96 (1).png��5�   *   $ Theme��5�   *   {�. .@�5�      
  -+   5        $ bthThemeDefualt��5�   +   $  Images\icons8-minus-96.png��5�   *   $ White Theme��5�   *   {�. $	��Top=�6O   6g   
  5�      -,   5        $ btnThemeDark��5�   +   $  Images\icons8-minus-96.png��5�   *   $ 
Dark Theme��5�   *   {�. $	��Top=�6N   6f   5�      
  5�      --   5        $ btnChangeColor��5�   '   $  Images\icons8-minus-96.png��5�   *   $ Change Color��5�   *   {�. $	��Top=�6P   6h   
  5�      -.   5        $ btnCheckError��5�   '   $ Images\icons8-eye-96.png��5�   *   $ Check Error��5�   *   {�. $	��Top=�6O   6g   5�      5�      5�      5�      �� 5�   
   -/   5         ��Name$ lstSKU��5�       %� IsVirtualizing .  $��Stretch=�6R   $��0,138,-0.2,0.4q�6p   $��2��6�   $ 0.5,0.5&�6  z�5�      V� 5�      .~�5�      �� �� 5�      �� 5�      T� 5�      $	 -20Q�6)   5�        � ContextMenu  5�      �� 5�      .@�5�      m� -0   5        ! m�Name$! 
cmbRefresh��5�      $��Refresh��64   " m� InputGestureText$" F5��6E   6[   �� 5�      m� 5�      -1   5        $! cmbNav��5�       $��Navigate��60   {�.  .@�5�      m� 6%   m� 5�      6%   5�      �� 5�      m� 5�      -2   5        $! 
cmbLoadExe��5�      $��Load executable��63   {�.  6L   m� 5�      $��Load / Launch��6   .@�5�      m� $��Load target executable��6#   6E   m� 5�      $��Launch application...��6#   .@�5�      m� $��Run...��6'   69   �� 5�      m� 5�      $%��No installed applications found��6'   6R   5�      5�      m� 5�      -3   5        $! cmbScreenshot��5�      {�.  $��Take Screenshot��6H   6d   �� 5�      m� 5�      -4   5        $! 	cmbReboot��5�      {�.  $��Reboot��6D   6p   m� 5�      -5   5        $! 
cmbPowerOn��5�       {�.  $��Power on��6F   6t   m� 5�      -6   5        $! cmbPowerOff��5�      {�.  $��	Power off��6F   6v   m� 5�      -7   5        $! cmbPowerRest��5�      {�.  $��	Rest mode��6G   6x   �� 5�      m� 5�      -8   5        $! 
cmbConnect��5�      $��Connect��63   6`   m� 5�      -9   5        $! cmbDiscconect��5�      {�.  $��
Disconnect��6H   6{   �� 5�      m� 5�      -:   5        $! cmbSetFileSerDir��5�      $#��Set file serving directory...��69   6�   m� 5�      -;   5        $! cmbSetDefualt��5�      $��Set default��66   6j   �� 5�      m� 5�      -<   5        $! cmbImportTargetSettings��5�      $��Import target settings...��6@   {�.  6�   m� 5�      -=   5        $! cmbExportargetSettings��5�      $��Export target settings...��6?   {�.  6�   �� 5�      m� 5�      ->   5        $��System update...��5�      {�.  6d   m� 5�      -?   5        $��Activate...��5�      {�.  6a   �� 5�      6"   m� 5�      -@   5        $! 	cmbDelete��5�      $��Delete��62   $" Delete��6C   {�. 6�   m� 5�      -A   5        $! 	cmbRename��5�      $��Rename��62   $" F2��6C   {�.  6�   �� 5�      6"   m� 5�      -B   5        $! cmbInstallPkg��5�      $��Install package��66   {�.  6�   m� 5�      -C   5        $��Uninstall package...��5�      {�.  6k   m� 5�      -D   5        $"��Manage installed packages...��5�      {�.  6s   m� 5�      -E   5        $&�� Manage installed entitlements...��5�      {�.  6w   �� 5�      6"   m� 5�      -F   5        $! cmbCopyFiles��5�      $��Copy file(s)...��65   {�.  .@�5�      m� 6%   5�      �� 5�      6"   m� 5�      -G   5        $! 	cmbPlayGo��5�      $��PlayGo��62   {�.  .@�5�      m� 6%   5�      �� 5�      6"   m� 5�      -H   5        $! cmbKillProcess��5�      {�.  $��Kill Process��6I   .@�5�      m� 6%   5�      �� 5�      6"   m� 5�      -I   5        $! cmbCopy��5�      $
��Copy��60   {�.  $" Ctrl+C��6P   5�      �� 5�      6"   m� 5�      -J   5        $! cmbProperties��5�      {�.  $��
Properties��6H   5�      5�      # �� View# 5�      �� 5�      .P�5�      �� $ �� Width$
$ Auto��6%   $��Default��62   % �� DisplayMemberBinding% 6C   �� *	Default+�� 5�      $
$ Auto��6%   $��Target��62   % 6B   �� *Target+�� 5�      $
$ Auto��6%   $	��SDK��62   % 6@   �� *SDK+�� 5�      -K   5        actualStatus��$
$ Auto��5�   ;   $��Status��6H   % 6X   �� *Status+�� 5�      $$ 0��6&   -L   5        StatusProgress��$��Status��5�   J   & �� CellTemplate& 5�      �� 5�      .R�5�   "   � .+�5�   &   � ' �Name$' pbstatus��6D   
����  �( � Value( 6V   �� *
Progress+��6q   �� *Height+) �� ElementName$) StatusProgress����6�   �� *Width+$) StatusProgress���� 5�   &   $��Center=�60   $��Center=�6M   
���   ���6{   �� *ProgressName+5  '   5  #   5     5     �� 5     $
$ Auto��6%   $��Address��62   % 6D   �� *	Address+�� 5     $
$ Auto��6%   $��Power��62   % 6B   �� *Power+�� 5     $
$ Auto��6%   $��File serving directory��62   & 5	     �� 5
     .R�5  "   �� �� 6-   -M   5        ��5  7   �� *
FileSerDir+.�6�   �� .F�5  /   �� ��69   �� *
FileSerDir+5  ,   6@   6L   5     5     �� 5     $
$ Auto��6%   $��Expiration Time��62   % 6K   �� *Expire+�� 5     $
$ Auto��6%   $��Release Check mode��62   % 6N   �� *	Release+5     5     5     5     