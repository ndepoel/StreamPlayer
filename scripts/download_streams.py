from __future__ import print_function
import sys, argparse
import os, os.path
import tempfile
import paramiko
import re
from datetime import datetime
import pytz
import hashlib
import getpass

argparser = argparse.ArgumentParser(description='Tool to automatically move video stream files from an SFTP server to a local disk and retrieve each file\'s timestamp with millisecond precision.')
argparser.add_argument('-s', '--hostname', help='Hostname of the server to retrieve media files from.', required=True)
argparser.add_argument('-u', '--username', help='Username to log into the server. This user needs to be a sudoer.', required=True)
argparser.add_argument('-p', '--password', help='Password to log the user into the server with.')
argparser.add_argument('-m', '--media_path', help='Pathname on the remote host to retrieve the media files from. This needs to be on an ext4-formatted file system.', required=True)
argparser.add_argument('-o', '--output_dir', help='Local directory to download the media files to.', required=True)

class Connection(object):
    """Connects and logs into the specified hostname. 
    Arguments that are not given are guessed from the environment.""" 

    def __init__(self,
                 host,
                 username = None,
                 private_key = None,
                 password = None,
                 port = 22,
                 ):
        self._sftp_live = False
        self._sftp = None
        if not username:
            username = os.environ['LOGNAME']

        # Log to a temporary file.
        templog = tempfile.mkstemp('.txt', 'ssh-')[1]
        paramiko.util.log_to_file(templog)

        # Begin the SSH transport.
        self._transport = paramiko.Transport((host, port))
        self._tranport_live = True
        # Authenticate the transport.
        if password:
            # Using Password.
            self._transport.connect(username = username, password = password)
        else:
            # Use Private Key.
            if not private_key:
                # Try to use default key.
                if os.path.exists(os.path.expanduser('~/.ssh/id_rsa')):
                    private_key = '~/.ssh/id_rsa'
                elif os.path.exists(os.path.expanduser('~/.ssh/id_dsa')):
                    private_key = '~/.ssh/id_dsa'
                else:
                    raise TypeError, "You have not specified a password or key."

            private_key_file = os.path.expanduser(private_key)
            rsa_key = paramiko.RSAKey.from_private_key_file(private_key_file)
            self._transport.connect(username = username, pkey = rsa_key)

    def _sftp_connect(self):
        """Establish the SFTP connection."""
        if not self._sftp_live:
            self._sftp = paramiko.SFTPClient.from_transport(self._transport)
            self._sftp_live = True

    def get(self, remotepath, localpath = None, callback = None):
        """Copies a file between the remote host and the local host."""
        if not localpath:
            localpath = os.path.split(remotepath)[1]
        self._sftp_connect()
        self._sftp.get(remotepath, localpath, callback)

    def put(self, localpath, remotepath = None):
        """Copies a file between the local host and the remote host."""
        if not remotepath:
            remotepath = os.path.split(localpath)[1]
        self._sftp_connect()
        self._sftp.put(localpath, remotepath)

    def listdir(self, remotepath = None):
        """Lists the contents of a remote directory path."""
        if not remotepath:
            remotepath = '.'
        self._sftp_connect()
        return self._sftp.listdir(remotepath)

    def removefile(self, remotepath):
        """Removes a file from the remote host."""
        self._sftp_connect()
        self._sftp.remove(remotepath)
        
    def stat(self, remotepath):
        """Retrieve information abotu a file on the remote system."""
        self._sftp_connect()
        return self._sftp.stat(remotepath)
        
    def execute(self, command):
        """Execute the given commands on a remote machine."""
        channel = self._transport.open_session()
        channel.exec_command(command)
        output = channel.makefile('rb', -1).readlines()
        if output:
            return output
        else:
            return channel.makefile_stderr('rb', -1).readlines()

    def close(self):
        """Closes the connection and cleans up."""
        # Close SFTP Connection.
        if self._sftp_live:
            self._sftp.close()
            self._sftp_live = False
        # Close the SSH Transport.
        if self._tranport_live:
            self._transport.close()
            self._tranport_live = False

    def __del__(self):
        """Attempt to clean up if not explicitly closed."""
        self.close()

def md5sum(fname):
    hash_md5 = hashlib.md5()
    with open(fname, "rb") as f:
        for chunk in iter(lambda: f.read(4096), b""):
            hash_md5.update(chunk)
    return hash_md5.hexdigest()
        
prev_mbs = -1

def print_progress(count, total):
    global prev_mbs

    mbs = count / 1048576
    if mbs < prev_mbs:
        prev_mbs = -1
    
    if mbs > prev_mbs:
        total_mbs = total / 1048576
        progress = round(float(count * 100) / total)
        print("\r{} of {} MB ({}%)".format(mbs, total_mbs, int(progress)), end='')
        prev_mbs = mbs
        
def main(serverhost, username, password, mediapath, output_dir):
    # Connect to the SFTP server
    myssh = Connection(serverhost, username=username, password=password)

    # Figure out the disk device that the media path is located on
    remotedevice = None
    for line in myssh.execute('df --output=source {} | grep ^/'.format(mediapath)):
        remotedevice = line.strip()
        
    if not remotedevice:
        print('Could not find the remote disk device that {} is located on!'.format(mediapath))
        myssh.close()
        sys.exit(1)
    
    print('{} is located on disk device {}'.format(mediapath, remotedevice))
    
    # Figure out the timezone that the server is set to
    servertz = None
    for line in myssh.execute('timedatectl | grep -i zone'):
        match = re.search(r'one:\s+([^\s]+)\s+\(', line)
        if match:
            servertz = match.group(1)
            
    if not servertz:
        print('Could not find the timezone that the server is set to!')
        myssh.close()
        sys.exit(1)
        
    print('Server is set to timezone: {}'.format(servertz))
    
    # Get a list of FLV movie files from the media folder on the remote host
    remote_files = myssh.listdir(mediapath)
    print('Found {} file(s) to download in directory {}'.format(len([f for f in remote_files if f.endswith('.flv')]), mediapath))
    
    for remote_file in remote_files:
        remotepath = mediapath + '/' + remote_file
        (_, ext) = os.path.splitext(remotepath)
        if ext != '.flv':
            continue
            
        print("\nFile: " + remotepath)
        
        # Retrieve the remote file's creation time with nanosecond precision from the server
        crtime = None
        cmd = 'echo \'{}\' | sudo -S debugfs -R "stat <$(stat -c %i {})>" {} | grep crtime'.format(password, remotepath, remotedevice)
        for line in myssh.execute(cmd):
            match = re.match(r'crtime: 0x([0-9a-f]+):([0-9a-f]+)', line)
            if match:
                seconds = int(match.group(1), 16)
                nanosecs = int(match.group(2), 16) / 4
                crtime = datetime.fromtimestamp(seconds, pytz.timezone(servertz))
                crtime = crtime.replace(microsecond = nanosecs / 1000)
                break
                
        if not crtime:
            print('Failed to retrieve file creation time!')
            continue
                
        print('Creation time: ' + str(crtime))

        # Determine the local filename for this file, which includes milliseconds in the timestamp
        (filename, ext) = os.path.splitext(remote_file)
        match = re.search(r'(\w+)-(\d{4})(\d{2})(\d{2})-(\d{2})(\d{2})(\d{2})', filename)
        if not match:
            print('Filename does not match the expected pattern!')
            continue
            
        local_filename = os.path.join(output_dir, match.group(1) + '-{}-{:03}'.format(crtime.strftime('%Y%m%d-%H%M%S'), crtime.microsecond / 1000) + ext)
        
        # Calculate the remote file's MD5 sum so we can verify the locally downloaded version later
        remote_md5sum = None
        print('Calculating MD5... ', end='')
        for line in myssh.execute('md5sum ' + remotepath):
            match = re.match(r'(\w+)\s', line)
            if match:
                remote_md5sum = match.group(1)
                break
        
        if not remote_md5sum:
            print('Failed to calculate MD5 checksum!')
            continue
        
        print(remote_md5sum)
        
        # Download the file if it doesn't exist locally or is incomplete
        if not os.path.exists(local_filename) or md5sum(local_filename) != remote_md5sum:
            print('Downloading {} to local file: {}'.format(remote_file, local_filename))
            myssh.get(remotepath, local_filename, print_progress)
            print('')
        
        # If the file was already previously downloaded by its original name, rename it
        if os.path.exists(remote_file):
            print('Renaming {} => {}'.format(remote_file, local_filename))
            os.rename(remote_file, local_filename)
            
        # Delete the file from the server if it was succesfully downloaded
        if md5sum(local_filename) == remote_md5sum:
            print('Deleting remote file: {}'.format(remotepath))
            myssh.removefile(remotepath)

    myssh.close()

# start the ball rolling.
if __name__ == "__main__":
    args = argparser.parse_args()
    password = args.password if args.password else getpass.getpass()    
    main(args.hostname, args.username, password, args.media_path, args.output_dir)