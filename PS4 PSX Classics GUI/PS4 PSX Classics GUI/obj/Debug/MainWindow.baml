   M S B A M L   `   `   `  ����O  KPS4 PSX Classics GUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:PS4_PSX_Classics_GUIPS4_PSX_Classics_GUI  5   	   F BPulseButton, Version=1.0.3.0, Culture=neutral, PublicKeyToken=nullH3clr-namespace:NMT.Wpf.Controls;assembly=PulseButtonNMT.Wpf.Controls 5   	   U QVlc.DotNet.Wpf, Version=2.2.1.0, Culture=neutral, PublicKeyToken=84529da31f4eb963G4clr-namespace:Vlc.DotNet.Wpf;assembly=Vlc.DotNet.WpfVlc.DotNet.Wpf 5   	   %    PS4_PSX_Classics_GUI.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   4local"clr-namespace:PS4_PSX_Classics_GUI     5   	   Hcontrols3clr-namespace:NMT.Wpf.Controls;assembly=PulseButton     5   	   DVlc4clr-namespace:Vlc.DotNet.Wpf;assembly=Vlc.DotNet.Wpf     5   	   -   5          � Title$  PSX Classics GUI ��5
   	   $	��480��6#   $	��960��60    � Icon$ Resources/favicon.ico��6<    � WindowStartupLocation$ CenterScreen=�6p    � 
ResizeMode$ NoResize=�6�    � SizeToContent$ WidthAndHeight=�6�   .��5      � -   5         �Name$ 	Main_Grid��5      $��0,0,0,0q�6   |�. -  'System.Windows.Input.KeyboardNavigation   DirectionalNavigation$
 None=�6@   .+�5   
   ]� -   5         ]�Name$ BackgroundImage��5      ��5      �� 5       �� ImageSource$0 *Resources/45978933631_e9358fa25e_b (1).png��6   	 J� OpacityMask	 5      �� 5      $0 *Resources/45978933631_e9358fa25e_b (1).png��6   .+�5      o� $	��Top=�6   .@�5      m� -   5        $
��File��5      .@�5      m� -   5        $��Settings��5      61   m� 5      -   5        $��Log an issue��5      65   �� 5      m� 5      -   5        $��Add Custom PSX config��5      6>   �� 5      m� 5      -   5        $��Credits��5      60   5      5      � 5       .+�5!      � -	   5        $ 
homescreen��5!      $	��840��6+   $
��Left=�67   $	��450��6m   $��	0,0,0,0.4q�6z   $��Bottom=�6�   .+�5"      7  1System.Windows.Forms.Integration.WindowsFormsHost  -
   5        
  Name$
 WindowsFormsHost��5"   �   $��355,127,0,0q�6'   
 � Tag$ 0��6>   $
��Left=�6F   $	��175��6a   $	��175��6n   $	��Top=�6{    J� 	AllowDrop . $y�Visible=�6�   6�   �� 5#      -   5         ��Name$ Icon0��5#      $ 1��6+   $
��Left=�63   $	��175��6N   $	��175��6[   $��355,127,0,0q�6h   $	��Top=�6}    . $��Resources/icon0.png��6�    � ContextMenu 5$      �� 5%      .@�5&   "   m� -   5        $��Change Icon��5&   +   ��5'   &   �� 5(   *   5*   #   m� 5,   "   -   5        $��Change Background��5,   +   ��5-   &   �� 5.   *   50   #   51      54      �� 57      -   5         ��Name$ homescreen_dimmer��57       $��	0,0,0,610q�6;    . ��58      �� 59       �� 
StartPoint$ 0.5,0&�62    �� EndPoint$ 0.5,1&�6E   .��5:   "   �  � Color$ 	#88FFFFFF��6/    � Offset$	 0.0Q�6A   � 5;   "   $ 	#88FFFFFF��6/   $
 0.75Q�6A   � 5<   "   $ 	#00FFFFFF��6/   $	 1.0Q�6A   5=      5?      �� 5@      -   5        $ titlename_dimmer��5@       $	��686��6:   $��50��6F    . $��418,360,183,310q�6c   ��5A      �� 5B      $ 0.5,0&�62   $ 0.5,1&�6E   .��5C   "   � $ 	#40FFFFFF��6/   $	 0.0Q�6A   � 5D   "   $ 	#40FFFFFF��6/   $
 0.75Q�6A   � 5E   "   $ 	#00FFFFFF��6/   $	 1.0Q�6A   5F      5H      �� 5I      -   5         ��Name$ selecttitle��5I      $�� ��61   $
��Left=�6<   $��422,347,0,0q�6W   $	��Top=�6l    .  $��38(�6�   $��	Yu Gothic)�6�   ���	#FFFFFFFF$	��514��6�   $��54��6�   ��6�   Z� �� 5J      -   5         ��Name$ 
focuscolor��5J      ���	#00000000$��2q�6I   $
��Left=�6]   $	��233��6x   $	��168��6�   $��363,127,0,0q�6�   $	��Top=�6�   ._�5K      �� -   5         ��Name$ btnStart��5K   !   $�� ��63   $��-7.6,177.4,7.6,-2.6q�6>    .  $��20(�6m   $��	Yu Gothic)�6{   ���	#FFFFFFFF J� Opacity$	 0.7Q�6�   ���	#000051B66�   5L      �� 5M      ���	#BFFFFFFF$��2q�65   $
��Left=�6I   $	��175��6d   $	��175��6q   $��355,130,0,0q�6~   $	��Top=�6�   5N      �� 5O      $
��Left=�6   $��40��67   $��40��6C   $��	65,54,0,0q�6N   $	��Top=�6a   $��/Resources/psplus.png��6y   $y�	Collapsed=�6�   �� 5P      $
��Left=�6   $��40��67   $��40��6C   $��
577,52,0,0q�6N   $	��Top=�6b   $��/Resources/user.png��6z   $y�	Collapsed=�6�   �� 5Q      -   5        $ Username��5Q      $��User1��6.   $
��Left=�6>   $��
618,54,0,0q�6Y   $	��Top=�6m   ���	#FFFFFFFF$��16(�6�   5R      � 5S      -   5        $ OptionS��5S      
���   �$
 0.71Q�6V   $	��390��6e   $��Right=�6q   $��Stretch=�6�   .+�5T      �� -   5        $ PSXTitlelbl��5T      $��PSX BIN/CUE:��61   $
��Left=�6H   $��0,42,0,0q�6c   $	��Top=�6u    .  $��15(�6�   $��	Yu Gothic)�6�   ���	#FFFFFFFF��6�   Z� �� 5U      -   5         ��Name$ txtPath��5U      $�� ��6/   $
��Left=�67   $��	10,76,0,0q�6R   $	��Top=�6e    .  $��20(�6�   $��Yu Gothic UI)�6�   ���	#FFFFFFFF$��40��6�   $	��250��6�   ��6�   Z� 6�   �� 5V      -   5        $ btnSelectISO��5V      $�� ��63   $
��Left=�6>   $��
265,76,0,0q�6Y   $	��Top=�6m   $��53��6�   $��40��6�   
���   ���5W      �� 5X      $ Resources/psxicon.ico��6)    j� Stretch$ UniformToFill=�6M   5Z      �� 5[      -   5        $ lblPSXID��5[      $��PSX DISC ID:��6.   $
��Left=�6E   $��191,116,0,0q�6`   $	��Top=�6u    .  $	��15 (�6�   $��	Yu Gothic)�5\      ���	#FFFFFFFF��61   Z� �� 5]      -   5        $ txtContentID��5]      $��	CRST00001��64   $
��Left=�6E   $��
10,150,0,0q�6`   $	��Top=�6t    .  $��20(�6�   $��	Yu Gothic)�6�   ���	#FFFFFFFF$��40��6�   $	��380��6�   ��6�   Z� 6�   �� 5^      -   5        $ txtTitle��5^      $
��Left=�60   $��	0,234,0,0q�6K   $	��Top=�6^    .  $��20(�6�   $��	Yu Gothic)�6�   ���	#FFFFFFFF$��40��6�   $	��380��6�   ��6�   Z� 6�   �� 5_      -   5        $ 
lblNpTitle��5_      $��	NP Title:��60   $
��Left=�6D   $��
10,121,0,0q�6_   $	��Top=�6s    .  $��15(�6�   $��	Yu Gothic)�6�   ���	#FFFFFFFF��6�   Z� �� 5`      -   5        $ lblTitle��5`      $��Title :��6.   $
��Left=�6@   $��
10,195,0,0q�6[   $	��Top=�6o    .  $��15(�6�   $��	Yu Gothic)�6�   ���	#FFFFFFFF��6�   Z� �� 5a      $
��Left=�6   $	��Top=�68   $��
64,279,0,0q�5b      $��52��6%   $	��282��61   $y�Hidden=�6=   .��5c      ��  �� Orientation$ 
Horizontal=�6%   .+�5d      �� $��Resources/pkg.jpg��6$   $��48��6?   $��48��6K   �� 5e      $��Create PS2 Classic��6$   $
��Left=�6A    .  $��20(�6n   $��	Yu Gothic)�6|   ���	#FFFFFFFF6�   5f      5g      "  NMT.Wpf.Controls.PulseButton  5h      -   5          Name$ 
btnConvert��5h   +   $��49,308,72,90.4q�6@      
IsEllipsis .     IsReflective .    RadiusX$ 3Q�5k         RadiusY$ 3Q�5l          Pulses$  1��5m      !   	IsPulsing! . "   
PulseWidth$" 2��5o      #   
PulseSpeed$# 0:0:5D�5p      $   
PulseScale$	$ 1.2Q�5q      %   ButtonBrush
% ���� &   ButtonHighlightBrush
& ���� '   ButtonPressedBrush
' ��ح�(   
PulseColor
( �����.��5v      �� $ 
Horizontal=�6%   .+�5w      �� $��Resources/pkg.jpg��6$   $��48��6?   $��48��6K   �� 5x      $��Create PSX Classic��6$   $
��Left=�6A    .  $��20(�6n   $��	Yu Gothic)�6|   ���	#FFFFFFFF6�   5y      5z      �� 5{      -   5        $ btnMutePlaySound��5{      $�� ��67   $
��Left=�6B   $��
363,10,0,0q�6]   $	��Top=�6q   $��18��6�   $��18��6�   
���   �$y�	Collapsed=�6�   ��5|      �� 5}      $! Resources/icon_sound_on.png��6)   $ UniformToFill=�6S   5      5�      5�      5�      �� 5�   
   
���   �$��1q�6%   $
��Left=�69   $��15��6T   $��1202,-56,-262.4,0q�6`   $	��Top=�6{   $��14��6�   5�      5�      