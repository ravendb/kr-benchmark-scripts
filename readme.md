# Benchmark Scripts

This repository contains the benchmark scripts and the client applications for testing RavenDB performance on large dataset.

The raw data can be downloaded in `ravendbdump` format via bit.ly/3r40dTN (note 108GB file!)


Benchmark data files:
    - users.reqs - requests for the annotations of  the top 100K users
    - books.reqs - requests for annotations for the top 100K users on a particular book
    - userids.req - requests for user information on a particular user
        - This file isn't part of the repository, it is generated from the dataset here:
            https://www.reddit.com/r/datasets/comments/9i8s5j/dataset_metadata_for_69_million_reddit_users_in/
        - run `python3 scripts/gen_user_ids.py > userids.req` script to generate the file


Typical commands:

    ~/wrk2/wrk -L -R 250 -d 3m -c 128 -t 8 -s paths.lua http://172.31.36.34:5000  -- user.reqs

Run the benchmark on a particular client IP with the set of requests.

Benchmark is run for 3 minutes with 128 connections and 8 threads.
Note the `-R 250`, which indicates how many requests per second to generate.
The last argument select what kind of benchmark we'll run, can be any of the benchmark data files.

