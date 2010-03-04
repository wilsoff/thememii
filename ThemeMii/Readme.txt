ThemeMii is a manager for Wii Themes.
The .NET Framework 2.0 is required to run the application.

For more information visit the project page: http://thememii.googlecode.com
Please report bugs and make suggestions at the issue tracker there.



Changelog:
----------

Version 0.4
	- Fixed bricking CSMs when contents doesn't exists in original app
	- Fixed installing to nand backup

Version 0.3
	- Added a little tutorial on how to change the health screen (in the ?-menu)
	- Added ability to drag a mym file onto the window to open it
	- Window location and size will now be saved
	- Added option to save the nand backup path
	- Added option to take image width and height from the selected TPL
	- Fixed some bugs when saving/creating mym
	- Fixed "No Entries Left" error when files have no extension
	- Fixed base app downloading to different locations than the app directory
	- Fixed base app browsing

Version 0.2
	- Made the window resizable
	- Added some basic instructions "? -> Help"
	- Added ability to install a theme directly to a nand backup (Tools menu)
	- Added option to Lz77 compress containers for smaller csm's (enabled by default!)
	- Added option to keep the extracted base app (enabled by default!)
	- Added save prompt when closing application (can be turned off)
	- Added I4, I8, IA4 and IA8 as TPL formats (not compatible with MyMenu!!!)
	- Fixed TPL formats (all images were RGB5A3 before, didn't matter what you selected)
	- Fixed a bug when downloading base app
	- Fixed previewing of Lz77 compressed non-tpl images (base app browsing)
	- csm to mym: Fixed errors with Yaz0 compressed files
	- csm to mym: Fixed forgotten containers in mym.ini

Version 0.1
	- Initial Release

Thanks:
-------
icefire / Xuzz for the original MyMenu (and everything related)
ic#code for #ziplib
crediar for ash.exe

License:
--------
Copyright (C) 2009 Leathl
 
ThemeMii is free software: you can redistribute it and/or
modify it under the terms of the GNU General Public License as published
by the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

ThemeMii is distributed in the hope that it will be
useful, but WITHOUT ANY WARRANTY; without even the implied warranty
of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.