# Dyanamics-Plugin-Starter-pack
A starter pack to get started with making plugin and web resource for microsoft dynamics 365 platform.

**Setting it up**
 * The first thing to do while setting up this starter pack is to change the name of both the solution and the projects based on your own prefrence.
   * Use find and replace feature to change all instances of OrganizationName to something else like HopeFundation.
   * Then go to properties of each project and update the assembly name and default name.
   * Now, update both the project, solution and folder names.
   
 * Set up plugin registration tool.
   * in the project OrganizationName.PluginRegistration create a App.config. 
   * Copy and paste the content of AppExample.config in it.
   * Update the password, username and dynamics instance url.
   * Also update the .dll location. This is the location of the .dll file that is created when you build OrganizationName.Dynamics.Plugin.
   
   
  **Register the plugins to dynamics** 
   * One can easily register the plugins to dynamics with the help of the console application that already exist within this solution named Organizationname.PluginRegistration.
   * Just set it up as the startup project for the solution.
   * After setting up all the credentials in App.Config you just need to run the console application.
   
