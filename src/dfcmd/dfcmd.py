import json
from datetime import datetime
import pprint
import os

def print_commands(dupe_list):
    keep = dupe_list[0]
    del dupe_list[0]

    print('# Keeping: {}'.format(keep['FQFN']))
    for rm in dupe_list:
        print('rm {}'.format(rm['FQFN']))


with open('/home/bflanders/Desktop/df.json', 'r') as dupe_json_file:
    dupes = json.load(dupe_json_file)

for list in dupes:
    for dupe in list:
        dupe['SortTime'] = datetime.strptime(dupe['CreationTime'][0:18], "%Y-%m-%dT%H:%M:%S")
newdups = []
for list in dupes:
    sorted_on_location = sorted(list, key=lambda item: item['FQFN'], reverse=False)
    newdups.append(sorted(sorted_on_location, key=lambda item: item['SortTime'], reverse=True))

for list in dupes:
    print_commands(list)
    print("")

 