# SearchCopyTool
For oneself researching and using
Version 1.00 
	初版，完成基本搜索功能
Version 1.10
	更新DataGridView替换TextBox
	重新布局优化操作流程
Version 1.11
	增加DataGridView列以显示文件类型
	修改删除选定行，只有右键在DataGridView中选定行上才显示
	修改提示文本框及字体大小使其更加醒目
	修改查找为搜索，保持统一
	修改提示文本颜色
	增加搜索出的条目数量
	更换图标
Version 1.12
	更换图标
	可以按照关键字和类型排序
	可以仅拷贝选中文件
	开始搜索和开始拷贝按钮位置和大小调整
Version 1.13
	从VS2005更换到VS2015
	修正变量名
	使用list存储搜关键字
Version 1.14
	使用函数来进行功能分块设计，明确各个任务的目的
	修改搜索流程，使代码可读性增加
Version 1.15
	使用线程方式来搜索文件，防止程序假死,并在关闭时终止子线程，防止异常发生
	修改读取文件夹方式，当文件夹为保护属性时停止读取防止异常发生
