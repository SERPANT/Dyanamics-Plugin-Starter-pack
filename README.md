# Dyanamics-Plugin-Starter-pack
A starter pack to get started with making plugin and web resource for microsoft dynamics 365 platform.

**_Setting it up_**
 * The first thing to do while setting up this starter pack is to change the name of both the solution and the projects based on your own prefrence.
   * Use find and replace feature to change all instances of OrganizationName to something else like HopeFundation.
   * Then go to properties of each project and update the assembly name and default name.
   * Now, update both the project, solution and folder names.
   
 * Set up plugin registration tool.
   * in the project OrganizationName.PluginRegistration create a App.config. 
   * Copy and paste the content of AppExample.config in it.
   * Update the password, username and dynamics instance url.
   * Also update the .dll location. This is the location of the .dll file that is created when you build OrganizationName.Dynamics.Plugin.
   
   
**_Register the plugins to dynamics_**
   * One can easily register the plugins to dynamics with the help of the console application that already exist within this solution named Organizationname.PluginRegistration.
   * Just set it up as the startup project for the solution.
   * After setting up all the credentials in App.Config you just need to run the console application.
   
   
**_Plugin Development Overview_**

The basic skeleton for developing plugins has already been made. This includes the folder structure that need to be maintained to make development easier. Lets's go in detail about what each folder signifies. 
   * Constants: This folder is used to maintain all the constants that are used in the application like error messages, option set values etc.
     * EntityFields: This folder maintains the logical fields of an entity.
     * OptionSets: This folder maintains the integer values of option set used in the dynamics.
   * DI: This folder holds code for dependency injection. The code here does not have to be changed.
   * Entities: This is a folder that holds models.
   * Extentions: Hold utility files.
   * Plugins: holds the plugins themself. Each entity in dynamics has a folder here. Inside each of these entity folder we have 3 files depending on the stage in which the plugin gets executed (PostOperation, PreValidation, PreOperation).
   * ServiceAPI: This hold the interface for the services that we create.
   * Service : This holds all the services that we create.
