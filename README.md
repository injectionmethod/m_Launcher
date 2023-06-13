m_Launcher: ![image](https://github.com/injectionmethod/m_Launcher/assets/80434330/bc21c0b8-4e7c-4a45-9e32-7f24ea333f7b)


(Unreleased, Still In Development)

m_Launcher is a lightweight UI game launcher that simplifies the process of launching and managing games on Windows. With its user-friendly interface, m_Launcher provides an easy and intuitive way to access your favorite games. It supports Windows 7 through Windows 10 (Windows 11 compatibility not guaranteed).

![image](https://github.com/injectionmethod/m_Launcher/assets/80434330/a9b2285b-1d02-4c73-9736-36e4a8c36dc5)


FAQ
- Will i get banned from multiplayer games if i launch through this? | No, however the tools in the properties section could lead to a ban if not careful
- Can i launch Steam, Origin/EA, Epic, COG etc.. games with this? | Yes, you can launch any application from the exe and have it run through its regular client without setting anything
- Does this application connect to any online services? | Somewhat, The only connection this program will ever make provided it is from this repository is to the update server, it is turned off by default
- Why are certain parts of the UI unfinished? | This is a one man job, I don't have a lot of time to work on the UI with my schedule, hence it remains indev and my main focus till I can sort it out
- Will there be any social functionality, friends, voice chat etc? | It could easily be done but I dont think it fits with the model I would like, perhaps another application in the future might include this ;)
- I cant seem to figure out how to add applications, how do I do this? | Find the .exe within the games folder (can be tricky at times), drag and drop the exe into the application
- I cant run my game, it throws an error when i launch, what can I do? | Run the m_Launcher executable as administrator and explore the properties of the game via right click if it persists
- I cant understand why you cant just add a game importer, cant you do this? | I can, its being worked on but it is hard with my schedule, a few prototypes got scrapped but the current one is looking good
- Can I inject my hack.dll and not get caught | Yes, provided the game of choice hasnt got a solid hold on detecting LoadLibrary methods, since its open source you can customize the method
- Can I sell this? | Yes, I mean you do you chief but I dont know who would buy it.
- What are the thread priorities? | Thread priority is next to useless on modern machines however on older computers they help out a lot, see here: https://shorturl.at/nvyDW


Features
- Memory Injection: m_Launcher enables external debugging and game modding through memory injection techniques. This feature allows users to modify game behavior and explore the internals of running applications.

- Self-Installation: The entire m_Launcher application can be installed using just the executable file. No additional installation steps or dependencies are required, making it hassle-free for users to set up.

- Game Backup: Backing up your games is made simple with m_Launcher. You can easily create and manage backups of your games, ensuring that your progress and settings are safely stored.

- Memory Reading: Similar to Cheat Engine, m_Launcher provides memory reading capabilities for applications launched through it. This allows users to analyze and manipulate in-memory data of running games and applications.

- Open Source: m_Launcher is an open-source project, which means the source code is publicly available. This encourages community contributions and allows users to customize and improve the launcher according to their needs.

Usage
Installation: Simply run the m_Launcher executable to install the launcher. No additional steps are required.

Launching Games: After installation, launch m_Launcher, drag/drop the games executable and select it from the UI. Right click and select the "Play" button to start the game.

Memory Injection: To utilize memory injection for external debugging or game modding, refer to the provided documentation on how to interact with the memory of running applications.

Game Backup: Use the backup feature within m_Launcher to create and manage backups of your games. Follow the instructions in the user interface to perform game backups.

Contributing
We welcome contributions from the community to improve m_Launcher. If you have any suggestions, bug reports, or feature requests, please open an issue in the GitHub repository. Additionally, if you'd like to contribute code changes, feel free to submit a pull request.
