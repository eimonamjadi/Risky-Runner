# EndlessRunner
Endless Runner Research Game - USC Biokinesiology & Physical Therapy


Introduction


This game is developed as a part of research conducted by the USC Biokinesiology & Physical Therapy department. It utilizes the Unity3D game engine, specifically version Unity 2021.3.3f1, for both PC and Android platforms. The primary aim is to study human responses to various terrains and obstacles in a virtual environment, focusing on walking/running at different speeds.


Gameplay


The game is an endless runner where the player navigates through randomly generated levels. There are two distinct modes available, each designed to test various aspects of human biomechanics and cognitive response to environmental changes.


Research Purpose


Our objective is to understand how different speeds and terrains affect player performance and decision-making in-game. This research will contribute valuable insights into human biomechanics, potentially influencing physical therapy practices and the design of rehabilitative game environments.

Directories

Builds- Contains PC and Android builds of early prototypes

EnlessRunner- Contains all files associated with Unity project

ProjectSettings- Contains crucial configuration files for audio, input, physics, graphics, and other settings essential for managing Unity projects.

UserSettings- Contains user-specific configurations, including editor settings, search preferences, and custom layouts for the Unity development environment.

To download and open the EndlessRunner Unity project, follow these steps:

1. Download the Project Files:
   - Download the ZIP file containing the project files.
   - Go to the project's repository page.
   - Click the green `Code` button, then select `Download ZIP`.
   - Extract the contents of the ZIP file to a directory of your choice.

2. **Install Unity Hub:**
   - If you haven't already, download and install Unity Hub from the official [Unity website](https://unity.com/).

3. Open Unity Hub and Add the Project:
   - Open Unity Hub.
   - Click the "Projects" tab on the left side.
   - Click the "Add" button located in the top right corner.
   - Navigate to the directory where you extracted the EndlessRunner project.
   - Select the folder that contains the project's `Assets` folder and click "Select Folder."

4. Open the Project:
   - The EndlessRunner project should now appear in your list of projects in Unity Hub.
   - Click on the project name to open it in Unity.

5. Configure the Unity Version:
   - Unity Hub will prompt you to install the correct version (specifically version 2021.3.3f1) of Unity if it is not already installed. Follow the prompts to download and install the required version.
   - Once installed, Unity will open the EndlessRunner project.

6. Run the Project:
   - In the Unity Editor, open the "Scenes" folder in the Project window.
   - Double-click the main scene (Assets/Scenes/LevelSelection.unity) to open it.
   - Click the "Play" button at the top center of the Unity Editor to run the project.

By following these steps, you should be able to successfully download, open, and run the EndlessRunner Unity project.



Code Files

Unity script, `Loader`, manages level loading and player settings, including speed adjustments and session IDs, across different game levels like 'FixedSpeedLevel' and 'SelfPacedLevel'. It also handles data persistence and reporting for gameplay metrics and settings via Google Forms integration.

Unity script, `ScoreDisplay`, manages the game score display and penalties. It dynamically updates scores based on player collisions and time spent off the track, applying calculated penalties and updating the UI. The script also includes visual feedback by changing text color and style in different game states.

Unity script, `StumbleCount`, tracks and displays the number of stumbles a player incurs during gameplay. It updates the count and UI dynamically, while also triggering a visual change in the score display to reflect penalties. The script listens to stumble events from player controllers and adjusts its behavior based on whether the player uses the `PlayerController` or `NewPlayerController`.

Unity script, `SpeedSlider`, is part of a singleton class that manages the game's speed slider UI component. It smoothly transitions the slider value to reflect changes in game speed, using a coroutine for interpolated adjustments. The script ensures that speed changes are visually represented on the slider within a defined duration, enhancing the player's interaction with game speed settings.

Unity script, `MetricsManager`, is designed to handle data recording for game metrics into CSV files. It provides functionality to write detailed gameplay metrics for both fixed level and self-paced game modes. The script dynamically generates reports containing comprehensive data points such as movement speed, score, finish time, obstacle interactions, and path deviations. It ensures that new reports are appended to existing files or creates new files if they don't exist, making it a robust solution for tracking game analytics across sessions.

Assets Summary:
The assets in the Unity project are a diverse collection of resources ranging from visual components like animations, models, and environmental designs, to audio files for sound effects and UI elements. These also include scripts for game functionality, prefabricated elements for efficiency, and special folders like Resources and StreamingAssets for runtime asset management.
Packages Summary:
The Unity project's packages enhance development with advanced tools for camera control, rendering, IDE integration, and visual scripting, as well as utilities for testing, performance profiling, and JSON handling. These packages support both 2D and 3D development and include essential Unity extensions for UI design, scriptable render pipelines, and version control integration.

Game for Students to use

Requirements


- Access to the Internet to play the game
- Google Colab for running Python code and analyzing data


Playing the Game

1.Visit: https://play.unity.com/mg/other/webgl-builds-386565

2.  For data analysis, access the provided Google Colab notebook and follow the instructions to run the Python code.

	
Upon starting the game for the first time, each player will be assigned a unique ID. This ID will help in tracking progress and data collection throughout the experiments. Players are encouraged to familiarize themselves with both game modes to fully understand the experiment's scope.


Data Collection and Analysis


Data from gameplay is automatically collected and stored in a Google Sheet. The provided Google Colab notebook allows for the analysis of this data, focusing on player performance, speed selection, and adaptability to game-generated noise.


Questions for Student Experiments


Experiment 1. Fixed Speed Trials: How does the average speed of the runner affect their average score?

Experiment 2. Experience and Performance: How does the total score change for a fixed speed level as the user gains more experience?

Experiment 3. Noise Influence: How does the performance of the user scale with the amount of noise generated by the game?

Experiment 4. Speed Selection: Do people choose a speed in the freely-selected speed condition that maximizes their score?

Experiment 5. Speed and Practice: Do people change their average speed with practice in the free-speed condition?


These questions are designed to guide students in exploring the data collected from gameplay, helping them understand the impact of different variables on human performance in a virtual environment.










