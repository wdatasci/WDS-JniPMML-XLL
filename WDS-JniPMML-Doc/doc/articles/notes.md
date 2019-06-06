---
uid: notes.md
title: Additional Usage Notes
---

## Additional Usage Notes

<ul>
<li>Accessing Java via JNICode creates a COM AddIn</li>
    Efforts have been made to make sure COM objects are clean up.
    However, should the process break for whatever reason, 
    there may be an Excel process hanging around.  In that case,
    look in the taskmgr's details. Or, use something like the 
    powershell snippets in the scripts folder to find and stop.

</ul>

