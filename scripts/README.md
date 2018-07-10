Installation
============

These instructions assume we're running on Windows. The Python scripts will work on Linux and MacOS as well, using similar commands.

  * Make sure Python is installed and available on the path
    * Ensure PIP is installed, if not run: `easy_install pip`
    * Ensure VirtualEnv is installed, if not run: `pip install virtualenv`
  * Create a virtual environment for the Scripts directory:
    * Open a command line and change to the Scripts directory: `cd [checkout path]\scripts`
    * Run: `[python path]\Scripts\virtualenv.exe .`
    * Activate the virtual environment using: `Scripts\activate.bat`
  * Install the dependencies using PIP:
    * With the virtual environment activated, run: `pip install -r requirements.txt`
  * Test that the scripts are functioning:
    * Run: `python download_streams.py`
    * Run: `python combine_streams.py`
    * Check that both scripts display usage instructions, without any errors

Usage
=====

To download the streams off of a Linux-based server over SSH:

  * Run: `python download_streams.py -s [hostname] -u [username] -m [remote media path] -o [local download dir]`
    * This will prompt for a password to login to the SSH server
    * To make the process non-interactive, you can add the password to the above command: `-p [password]`

The above command will download all .flv files from the remote media path, obtain their precise timestamps and add it to their filenames, 
compare the MD5 checksums of the locally downloaded files with the remote files, and delete the remote files if the checksums match.

To combine the downloaded videos into a series of split-screen collages:

  * Run: `python combine_streams.py -i [local download dir] -o [output dir] --ffprobe [path to ffprobe.exe] --ffmpeg [path to ffmpeg.exe] -f FreeSerif.ttf`
    * The script needs to know the location of ffprobe and ffmpeg to function. By default it assumes both files are available in directory: `ffmpeg\bin`
	* By default the videos are combined in arbitrary order and the audio tracks will be copied in the same order.
	  To specify a specific participant to give precedence (i.e. always placed first and leading for audio), add the argument: `-s [participant name]`
    * To skip encoding of video segments containing only a single stream, you can add the argument: `--skip-single`
	* To check what the script will do without actually doing the video encode, you can perform a dry run first by adding the argument: `--dry-run`
	
The videos are synchronized based on the timestamps in the filenames, so it is important that the files are not renamed before combining. Video segments shorter than 120 seconds are skipped.
The output videos are split up in segments based on the number of participants. If a participant enters or leaves, this results in a new video segment with the correct number of input videos combined.
Use the dry run feature to check ahead of time which input videos are detected, and how they will be split up into segments.