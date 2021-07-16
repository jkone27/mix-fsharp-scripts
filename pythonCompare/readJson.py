import json

# read file
with open('test.json', 'r') as myfile:
    data=myfile.read()

# parse file
obj = json.loads(data)

res = obj["hello"]

print(res)