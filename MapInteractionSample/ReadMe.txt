 _______  _______  _        _______ _________ _______  _______ 
(  ____ \(  ____ \( (    /|(  ____ \\__   __/(  ____ \(  ____ \
| (    \/| (    \/|  \  ( || (    \/   ) (   | (    \/| (    \/
| |      | (__    |   \ | || (__       | |   | (__    | |      
| | ____ |  __)   | (\ \) ||  __)      | |   |  __)   | |      
| | \_  )| (      | | \   || (         | |   | (      | |      
| (___) || (____/\| )  \  || (____/\   | |   | (____/\| (____/\
(_______)(_______/|/    )_)(_______/   )_(   (_______/(_______/

  ___ ___                __            __                       
 /   |   \_____    ____ |  | _______ _/  |_  ______  _  ______  
/    ~    \__  \ _/ ___\|  |/ /\__  \\   __\/  _ \ \/ \/ /    \ 
\    Y    // __ \\  \___|    <  / __ \|  | (  <_> )     /   |  \
 \___|_  /(____  /\___  >__|_ \(____  /__|  \____/ \/\_/|___|  /
       \/      \/     \/     \/     \/                       \/ 


In this sample, you will find everything you need to create a custom task, 
display a map, add custom control on the map and interact with those control.


Requirements:
- Security Center Client
- Security Center SDK


How to use Security Center SDK:
- Add the following dll to your project:
	* Genetec.Sdk.dll
	* Genetec.Sdk.Controls.dll
	* Genetec.Sdk.Workspace.dll
- Follow "Readme_SecurityDeskSDK.txt" to register your module


Certificates:
In order for your SDK application to connect to the Directory, 
you need a valid certificate file located at "<app directory>/Certificates".

This sample already contain two certificates. You can copy them
and rename the file with the namespace of your application.

For more information, read the following article from the SDK documentation:
Genetec.Sdk.chm --> Platform SDK > Setup Articles > Using certificates 


MapInteractionSample class description:

--> ModuleMapInteractionSample.cs
This is the most important class of this sample. Without this class
nothing will work. This is where our system will look to know what 
your application can do. You will register your task and provider there.

--> FireMapObjectProvider.cs
This is where you can manage what is displayed on a map.
The Query method override will be called everytime there is a change
to the map (zoom, pan, load, etc.). To update the map when there is a new
fire, you need to call Invalidate(...).

--> FireMapObjectView.xaml
This is the class that manage the display of an item on a map.

--> MapFireAnalysisPage.cs / MapFireAnalysisView.xaml
This is the task and the UI.
The view is simply a user control where you can display any control
you want. For our case we want a combo box to select a map, the map control and a list of fires
The page contain descriptors to allow us to know how to display the task,
which icon to use and how to name it.