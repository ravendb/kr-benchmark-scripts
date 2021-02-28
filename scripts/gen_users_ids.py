import gzip
import json

with gzip.open('/home/ubuntu/69M_reddit_accounts.csv.gz', 'rt') as f:
     first = True
     for line in f:
          if first:
             first = False
             continue
          part = line.split(',')[0]
          print("/databases/Library/docs?id=users/" + part)
