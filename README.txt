----------------------------
-- LaTeX Homework Spawner --
----------------------------

Version 2.0

Written by Ethan Levine


1. Setup
2. Usage
3. Configuration
  a. Configuration File
  b. Template File


1. SETUP
----------------------------

The LaTeX Homework Spawner has three main parts.  The first part is the
executable, "LaTeX Homework Spawner.exe".  The library "Newtonsoft.Json.dll"
must be in the same directory as the executable.  If it is not, then the
program will crash when you try to load it.

The second part is the template file.  The template file is a LaTeX source
file with special tags in it that let the spawner customize it for a specific
document.  This file can be safely edited without adjusting the program.  For
more details, see section 3.b.

The third part is the configuration file.  The configuration file is optional.
It is stored in JSON format.  For more information, see www.json.org.  The
default file contains an entry for every available option, and it should be
intuitive to understand.  More details about it cna be found in section 3.a.


2. USAGE
----------------------------
