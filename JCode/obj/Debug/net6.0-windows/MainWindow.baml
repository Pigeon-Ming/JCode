   M S B A M L   `   `   `  ????@  <JCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:JCodeJCode  5   	   )clr-namespace:JCode.ModelJCode.Model  5   	       JCode.MainWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   %localclr-namespace:JCode     5   	   ,local1clr-namespace:JCode.Model     5   	     ? Title$  JCode??5	   	   $	??450??6   $	??800??6$    ? 	Resources 5
       $   MainTabControl_RightButtonUpMenu?? 5   
   %  &	        ?? 5      .@?5      m? $3??-MainTabControl_RightButtonUpMenu_CopyFilePath??6+   -   5        $??复制文件路径??5      6`   ?? 5      m? 5      $0??*MainTabControl_RightButtonUpMenu_CloseFile??6'   -   5        $??关闭??5      6Y   5      5      .??5      ?  ? RowDefinitions 5   
   ?? 5      $
??auto ?6   ?? 5      $
??auto ?6   ?? 5      $??20 ?6   ?? 5      ?? 5      $??10 ?6   ?? 5      $
??0.4* ?6   $??28??6*   ?? 5      $
??auto ?6   .+?5   
   o? $	??Top=?6   .@?5      m? $??_文件??6   .@?5      m? -   5        $??Menu_File_Creat??5   (   $??_新建??6   6?   m? 5       -   5        $??Menu_File_Open??5    (   $??_打开??6   6>   m? 5!      -   5        $??Menu_File_Save??5!   (   $??_保存??6   6>   m? 5"      -   5        $??Menu_File_SaveAs??5"   )   $??
_另存为??6   6A   ?? 5#      m? 5$      -   5        $??Menu_File_Exit??5$   -   $??_退出JCode??6   6C   5%      m? 5&      $??生成??6   .@?5'      m? -   5        $??Menu_Build_BuildProgram??5'   )   $??生成程序??6   6H   5(      m? 5)      $??选项??6   .@?5*      m? -	   5        $ ??Menu_Setting_BuildSettings??5*   )   $??生成设置??6   6K   5+      m? 5,      $??帮助??6   .@?5-      m? -
   5        $??Menu_Help_DevDocument??5-   1   $??JigsawOS开发文档??6   6N   m? 5.      -   5        $??Menu_Help_AboutJCode??5.   ,   $??关于JCode??6   6H   5/      51      ?? 52   
   $??JigsawOS IDE??6   $??Right=?6(   $??0,0,8,0q?6D   
???????6U   `? 53   
   $??1??6   $??25??6#   .?54      c? .@?55      ?? -   5        $??新建??55      6&   ?? 56      -   5        $??打开??56      6&   ?? 57      -   5        $??保存??57      6&   ?? 58      -   5        $??生成??58      6&   5:      5;      ?? 5<   
   -   5        $??MainTabControl??5<   ?    ? ContextMenu# [  6   $??2??6?    ?? TabStripPlacement$	 Top=?6?   ??5=      ?? 5>       ?? DataType$ local1:LocalFile??6   .R?5?      ?? ??6    ?? *Name+5@      5B      ?? 5C   
   $??3??6   .??5D      ? -   5        $??EditControl??5D      $y?	Collapsed=?6&    ? ColumnDefinitions 5E      ?? 5F      $??33 ?6'   ?? 5G      .+?5I      ?? -   5        $??Content_Line??5I   S   $??Right=?6   ${?False??6A   $??0,0,0,0q?6g   $??Stretch=?6x   ?? 5J      -   5        $??Content_RichTextBox??5J   .   $??1??6    5K       ?? 
TargetType ?? Property	 ?? Value??%?   'J?       ?? 5L      " J?6    .?5M      ??  ?@??Margin$	 0q?67   ?? 5N       ?@??
LineHeight$	 8??6;   5O      5P      5Q      5S      5T      ?? 5U   
   $??5??6   $??Stretch=?6"   $??4??6@   ?? 5V   
   $??5??6   .@?5W      ?? $??输出??6   .??5X      ?? .??5Y      ?? -   5        $??OutPut_TextBox??5Y   D   
 ?? 
IsReadOnly$

 True??6    ?? TextWrapping$
 Wrap=?60   6D   5Z      5[      5\      5]      5^      