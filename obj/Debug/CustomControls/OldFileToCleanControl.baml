   M S B A M L   `   `   `  ����K  GMusicFileManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:MusicFileManagerMusicFileManager  5      ,    &MusicFileManager.OldFileToCleanControl   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      0localclr-namespace:MusicFileManager     5      -   5          >�Name$  ctrlMain��5   ;   $	��500��5   7   $	��500��6C   $	��500��6P   $	��500��6_    � 	Resources 5	      -   'MusicFileManager.DuplicateTypeConverter   DuplicateTypeConverter��%   &	          5
   
   6'   5      .��5      � -   5         �Name$ grdBack��5      ���	#B2000000.+�5   
   �� -   5         ��Name$ btnClear��5      $��Clear��6#   $��Right=�63   $��
0,0,100,10q�6O   $��75��6c   $��22��6n   $��Bottom=�6z   6�   &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ 
dgFileList��5      $��0q�6'   $	��440��62   $	��400��6>      AutoGenerateColumns .     ColumnHeaderHeight$ 20Q�6g      RowHeaderWidth$ 0Q�6    �� AlternationCount$ 1��6�   	   	RowHeight$	 25Q�6�   
 J� SnapsToDevicePixels
 . $��DotumChe)�6#     CanUserReorderColumns .     GridLinesVisibility$ 
Horizontal=�6�  
�������$��Center=�6�  $��Center=�6�     HorizontalGridLinesBrush 6�   �� *
Background+ �� ElementName$ grdBack�� �� Mode$ TwoWay=���6W  Z� ��6m  Z�  5       �� Color=  7System.Windows.Controls.Primitives.DataGridColumnHeader �� 
TargetType �� Property �� Value)  #System.Windows.Controls.DataGridRow �� Triggers P� Property P� Value*  $System.Windows.Controls.DataGridCell��%�  (��       5   "   * SystemColors.HighlightBrushKey+)'  !     '  �     '  �    �� 6   $ Transparent��6T   �� 5      "  6   .�5      ��  � �� �	#FF6DFFD163   �� 5       � ��$ Center=�6C   5      �� 5      "  6   .�5      ��  � ��
 ���� 63   �� 5       � ��
 ���� 64   �� 5       � ��$ 0q�68    5      P� 5       �@ 
IsSelected . . �5      ��  � ��
 ���� 6<   �� 5       � ��$ 0q�6@   5      5      �� 5       "  6    5!      P� 5"       �@ 
IsSelected . . �5#      ��  � ��
 ���� 6<   �� 5$       � ��$ 0q�6@   5%      5'      5(         Columns 5)      0  *System.Windows.Controls.DataGridTextColumn  5*      -   5        colOriginFile��,  &System.Windows.Controls.DataGridColumn   CanUserResize .     MaxWidth$	 150Q�5*   R      MinWidth$ 50Q�6a      Width5  /System.Windows.Controls.DataGridLengthConverter$	 100 6o      
IsReadOnly .    CanUserReorder .     Header$ 원본파일��6�   1	  +System.Windows.Controls.DataGridBoundColumn 	  Binding 6�   �� *OriginalFile+  5+      -   5        colFiletoClean�� .   . $	 150Q�5+   e   $ 50Q�6t   $	 100 6�    .  $ 정리파일��6�    6�   �� *FileToClean+  5,      -   5        colDuplicateType�� .  $	 300Q�5,   V   $	 100Q�6e   $	 190 6t   $ 중복유형��6�    6�   �� *DuplicateType+  �� 	Converter#  [  4
  .System.Windows.Controls.DataGridCheckBoxColumn
  5-      -   5        colHaveToClean�� .  $ 28 5-   W   $ 삭제��6b    6n   �� *HaveToClean+$ TwoWay=�5/      �� 50   
   -	   5        $ 	btnCancel��50      $��Cancel��6$   $��	0,0,10,10q�65   $��Right=�6H   $��75��6d   $��22��6o   $��Bottom=�6{   6�   52      53      