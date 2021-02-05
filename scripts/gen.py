import json

with open('annotations.json') as f:
    data = json.load(f)
    for i in data:
        print ('/annotations/user/0/10/?userId='+ str(i['id']) )