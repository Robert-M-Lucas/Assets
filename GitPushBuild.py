DIR = "C:\\Users\\rober\\Downloads\\Chess Build"
DEFAULT_MSG = """Build Update"""

import os
import subprocess


def show_output(p):
    p = p.communicate()[0]
    while True:
        line = p.stdout.readline()
        if not line: break


msg = input("Enter message (blank for 'Build Update'): ")

if msg == "":
    msg = DEFAULT_MSG

os.chdir(DIR)

p = subprocess.run(["git", "add", "*"], capture_output=True)
print(p.stderr.decode())
print(p.stdout.decode())

p = subprocess.run(["git", "commit", "-a", "-m", f'"{msg}"'], capture_output=True)
print(p.stderr.decode())
print(p.stdout.decode())

p = subprocess.run(["git", "push"], capture_output=True)
print(p.stderr.decode())
print(p.stdout.decode())

input("Done")
