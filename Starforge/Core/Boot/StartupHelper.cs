using Starforge.Editor;
using Starforge.Mod.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Starforge.Core.Boot {
    /// <summary>
    /// Responsible for loading required resources and performing startup tasks.
    /// </summary>
    public static class StartupHelper {
        /// <summary>
        /// Whether or not Starforge is currently starting up.
        /// </summary>
        public static bool CurrentlyStarting { get; private set; } = false;

        /// <summary>
        /// Whether or not loading has finished.
        /// </summary>
        public static bool Finished { get; private set; } = false;

        /// <summary>
        /// Whether or not the first stage of loading (all but textures) has finished.
        /// </summary>
        public static bool FinishedFirstStage { get; private set; } = false;

        /// <summary>
        /// The amount of finished startup tasks.
        /// </summary>
        public static int FinishedTasks { get; private set; } = 0;

        /// <summary>
        /// The total amount of startup tasks.
        /// </summary>
        public static int TotalTasks { get; private set; } = 0;

        /// <summary>
        /// Whether or not any startup tasks have errored.
        /// </summary>
        public static bool HasErrored { get; private set; } = false;

        /// <summary>
        /// The list of startup tasks which still need to finish.
        /// </summary>
        private static List<BootTask> UnfinishedTasks = new List<BootTask>();

        /// <summary>
        /// The list of startup tasks which still need to run.
        /// </summary>
        private static List<BootTask> UnstartedTasks = new List<BootTask>();

        /// <summary>
        /// The cancellation token to stop startup tasks.
        /// </summary>
        private static CancellationTokenSource CancellationToken = new CancellationTokenSource();

        /// <summary>
        /// Checks if the given folder appears to be a valid Celeste install.
        /// </summary>
        /// <param name="location">The path to check.</param>
        /// <returns></returns>
        public static bool IsCelesteInstall(string location) {
            if (Directory.Exists(location)) {
                if (
                    File.Exists(Path.Combine(location, "Celeste.exe")) &&
                    Directory.Exists(Path.Combine(location, "Content")) &&
                    Directory.Exists(Path.Combine(location, "Content", "Graphics"))
                ) {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Begins spawning threads responsible for loading game content, etc.
        /// </summary>
        public static void BeginStartupTasks() {
            if (CurrentlyStarting) return;
            CurrentlyStarting = true;

            // Find and run all startup tasks contained in Starforge
            Type[] asmTypes = Assembly.GetExecutingAssembly().GetTypes();
            foreach (Type t in asmTypes) {
                if (t.IsSubclassOf(typeof(BootTask))) {
                    RegisterTask((BootTask)Activator.CreateInstance(t));
                }
            }
        }

        /// <summary>
        /// Marks a given task as finished.
        /// </summary>
        /// <param name="task">The task to finish.</param>
        public static void Finish(BootTask task) {
            if (UnfinishedTasks.Contains(task)) {
                UnfinishedTasks.Remove(task);
                FinishedTasks++;
            }
        }

        /// <summary>
        /// Updates the status of currently running tasks.
        /// </summary>
        public static void Update() {
            foreach (BootTask task in UnfinishedTasks.ToArray()) {
                if (task.Task.IsFaulted) {
                    // Task threw an error.
                    HasErrored = true;
                    Logger.Log(LogLevel.Error, $"A startup task encountered an error.");
                    Logger.LogException(task.Task.Exception);

                    return;
                }
            }

            // If startup has errored - do not continue.
            if (HasErrored) {
                CancellationToken.Cancel();
                return;
            }

            // Start new tasks
            if (UnstartedTasks.Count > 0) {
                if (UnfinishedTasks.Count < Settings.MaxStartupThreads) {
                    int TasksToStart = Settings.MaxStartupThreads - UnfinishedTasks.Count;

                    while (TasksToStart > 0 && UnstartedTasks.Count > 0) {
                        BootTask newTask = UnstartedTasks[0];

                        UnfinishedTasks.Add(newTask);
                        UnstartedTasks.Remove(newTask);

                        newTask.Task.Start();
                    }
                }
            }

            // Finished loading - all tasks completed
            if (UnstartedTasks.Count == 0 && UnfinishedTasks.Count == 0) {
                FinishedFirstStage = true;
                CurrentlyStarting = false;
            }
        }

        /// <summary>
        /// Unpacks any texture atlases that needs to be loaded.
        /// </summary>
        public static void UnpackAtlases() {
            GFX.Gameplay = Atlas.FromAtlas(Path.Combine(Settings.CelesteDirectory, "Content", "Graphics", "Atlases", "Gameplay"), AtlasFormat.Packer);
            GFX.Empty = new DrawableTexture(GFX.Gameplay.Sources[0], 4094, 4094, 1, 1);
            GFX.Pixel = new DrawableTexture(GFX.Gameplay.Sources[0], 13, 13, 1, 1);
            GFX.Scenery = new Tileset(GFX.Gameplay["tilesets/scenery"], 8, 8);

            // PICO-8 font loading
            DrawableTexture font = GFX.Gameplay["pico8/font"];
            GFX.Font = new DrawableTexture[font.Width / 4 * (font.Height / 6)];
            for(int i = 0; i < font.Height / 6; i++) {
                for(int j = 0; j < font.Width / 4; j++) {
                    GFX.Font[j + i * (font.Width / 4)] = new DrawableTexture(font, j * 4, i * 6, 4, 6);
                }
            }

            Finished = true;
        }

        /// <summary>
        /// Registers a task to be run.
        /// </summary>
        /// <param name="task">The task to register.</param>
        private static void RegisterTask(BootTask task) {
            task.Task = new Task(new Action(task.Run), CancellationToken.Token);

            UnstartedTasks.Add(task);
            TotalTasks++;
        }
    }
}
