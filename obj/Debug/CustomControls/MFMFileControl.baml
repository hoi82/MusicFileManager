   M S B A M L   `   `   `  ����  MusicFileManager4    .MusicFileManager.CustomControls.MFMFileControl   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      -   5          >�Name$  userControl��5      .��5      � .+�5	   
   �� -   5         ��Name$ bdBorder��5	      ���	#FF282828 �� CornerRadius$ 10��6:   $��10q�6L    J� Effect 5
      3  -System.Windows.Media.Effects.DropShadowEffect  5         
BlurRadius$ 10Q�6      	Direction$ 0Q�6-      ShadowDepth$ 2Q�6;   ._�5      �� -   5        $ bdBackground��5      $ 8��6(   $��10,50,10,10q�69   ���	#FF323232._�5      ��    MFMScrollViewer#���   6   $
��Auto=�6F   .��5      � -   5         �Name$ wpMain��5      }�6'   �� *ClipToBounds+ �� ElementName$ userControl��	 � 
ItemHeight	 6f   �� *
ItemSize+$ userControl��
 �� Mode$
 OneWay=� � 	ItemWidth 6�   �� *
ItemSize+$ userControl��$
 OneWay=�5      5      5      �� 5   
   -   5         ��Name$ lbTitle��5      $
��Left=�6T   $	��Top=�6o   $
��Bold"�6�   $��30��6�   $��21.333(�6�   $��	20,20,0,0q�6�   $5��//MusicFileManager;component/Fonts/#Walkway Bold)�6  ��6!   �� *Title+$ userControl����6�   �� *
Foreground+$ userControl��$
 OneWay=� 5        5      $ 10Q�6   $ 0Q�6-   $ 2Q�6;   5      � 5   
   -   5         �Name$ popItem��5       � AllowsTransparency . $
��Fade=�6;   .��5      � -   5         �Name$ grdItemContent��5      .+�5      �� -   5        $ outerPopBorder��5      ���	#FF1E1E1E$ 10��6H   $��10q�6Z    5        5      $ 10Q�6+   ._�5      �� -	   5        $ innerPopBorder��5      ���	#FF323232$ 7��6L   $��5q�6]   ._�5      �� -
   5         ��Name$ tblItemContent��5   $   $��Center=�6<   $
��Wrap=�6Y   $��System.Object��6m   $��Center=�6�   ���	#FFFF6400$5��//MusicFileManager;component/Fonts/#Walkway Bold)�6�   $��10q�6(  ��6�   �� *
FontSize+$ userControl��6(  5       5!      5"      5#      5$      5%      