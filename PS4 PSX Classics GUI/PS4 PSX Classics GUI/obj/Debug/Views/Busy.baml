   M S B A M L   `   `   `  ����O  KPS4 PSX Classics GUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:PS4_PSX_Classics_GUIPS4_PSX_Classics_GUI  5   	   F BPulseButton, Version=1.0.3.0, Culture=neutral, PublicKeyToken=nullH3clr-namespace:NMT.Wpf.Controls;assembly=PulseButtonNMT.Wpf.Controls 5   	       PS4_PSX_Classics_GUI.Busy   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   4local"clr-namespace:PS4_PSX_Classics_GUI     5   	   Hcontrols3clr-namespace:NMT.Wpf.Controls;assembly=PulseButton     5   	   -   5          � Title$   ��5	   	   $	��480��6   $	��960��6    � WindowStartupLocation$ CenterScreen=�6+    � Icon$; 5/PS4 PSP Classics GUI;component/Resources/favicon.ico��6P   .��5
      � ��5   
   �� 5       �� ImageSource$8 2/PS4 PSP Classics GUI;component/Resources/flow.png��6   .+�5   
   ��  �� Orientation$ Vertical=�6   .+�5      �� -   5         ��Name$ lblTitle��5      $��
PSP HD Gui��6&   $��20(�6;   $��	Yu Gothic)�6I   ���	#FFFFFFFF$��	21,10,0,0q�6w   �� 5      $��27��6   $��0q�6$   
�������6/   �� 5      $ Vertical=�6   .+�5      �� -   5         ��Name$ lblTask��5      $	��134��6-   $��105,0,104.6,0q�6:   $��20(�6Q   $��	Yu Gothic)�6_   ���	#FFFFFFFF$	��744��6�   ��5      �� 5      5      � 5      $��20��6   $��105,0,104.6,0q�6*    � Value$ 50Q�6A   ���	#FFFFFFFF���#e5e5e5 � IsIndeterminate . 6x   �� 5      -   5        $ txtExtraInfo��5      $��89��62   $��105,0,104.6,0q�6>   $��20(�6U   $��	Yu Gothic)�6c   ���	#FFFFFFFF$	��744��6�   ��5      �� 5      5      �� 5      $ 
Horizontal=�6   .+�5%      "  NMT.Wpf.Controls.PulseButton  -   5        	  Name$	 btnPositive��5%   +   $��20(�6@   $��	Yu Gothic)�6N   ���	#FFFFFFFF$��	100,0,0,0q�6|   
   
IsEllipsis
 .     
PulseScale$	 1.2Q�5'         
PulseWidth$ 2��5(         
PulseSpeed$ 0:0:5D�5)         Pulses$ 1��5*         RadiusX$ 3Q�5+         RadiusY$ 3Q�5,      $	��Yes��5-         ButtonBrush
 ����    ButtonHighlightBrush
 ����    ButtonPressedBrush
 ��ح�   
PulseColor
 �����$��44��52      $	��200��6+   $y�Hidden=�67   �� 58      -   5         ��Name$ 	btnNutral��58      $y�Hidden=�60   $��Cancle��6D   $��44��6U   $	��200��6a   $��20(�6m   $��	Yu Gothic)�6{   ���	#FFFFFFFF$��Center=�6�   $��75,0,0,0q�6�   ��59      �� 5:      5<      �� 5=      -   5        $ btnNegative��5=      $y�Hidden=�62   $��No��6F   $��44��6S   $	��200��6_   $��20(�6k   $��	Yu Gothic)�6y   ���	#FFFFFFFF$��Center=�6�   $��70,0,0,0q�6�   ��5>      �� 5?      5A      5B      5E      �� 5F      $��27��6   $��0q�6$   
�������6/   �� 5G      $ 
Horizontal=�6   .+�5H      �� $
��Left=�6   $��30��63   $��30��6?   $��	680,0,0,0q�6J   $	��Top=�6]   $��/Resources/user.png��6u   �� 5I      -   5        $ Username��5I      $��User1��6*   $
��Left=�6:   $��0,0,0,0q�6U   $	��Top=�6f   ���	#FFFFFFFF$��16(�6�   5J      5N      �� 5O   
   -	   5         ��Name$ OptionsView��5O   )   $y�Hidden=�6   $ Vertical=�6>   
���   � J� Opacity$	 0.7Q�6h   $��714,0,-0.4,0.4q�6v   .+�5P      �� -
   5        $ LabelOn��5P      $��On��6%   $��Center=�62   $	��Top=�6O   $��0,212,0,211.6q�6g   $��20(�6~   $��	Yu Gothic)�6�   ���	#FFFFFFFF6�   �� 5Q      -   5        $ labelOff��5Q      $	��Off��6&   $��Center=�64   $��
0,-200,0,0q�6Q   $��20(�6e   $��	Yu Gothic)�6s   ���	#FFFFFFFF$	��Top=�6�   5R      5S      5T      