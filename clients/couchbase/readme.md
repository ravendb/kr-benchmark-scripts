# Setup

docker run -d --name db -p 8091-8096:8091-8096 -p 11210-11211:11210-11211 couchbase  
docker run -d -p 8091-8096:8091-8096 -p 11210-11211:11210-11211 couchbase

docker run -d -p 8091-8096:8091-8096 -p 11210-11211:11210-11211 couchbase:enterprise-7.0.0-beta

http://localhost:8091/

// Annotations/users/5101859-ebooks/56717/0000000002180997833-A
// Annotations/users/5101859-ebooks/56717/0000000002180997834-A

# Define Indexes

CREATE PRIMARY INDEX ON `Library`
select * from `Library`
SELECT META().id FROM `Library`

CREATE PRIMARY INDEX ON `default`:`Library`.`_default`.`Annotations`

SELECT * FROM `Annotations` where `user` = 'users/5101859'

SELECT * FROM Library._default.Annotations where `user` = 'users/5101859'

SELECT RAW a FROM Library._default.Annotations a where a.`user` = 'users/5101859'

SELECT RAW a FROM Library._default.Annotations a where a.`user` = 'users/5101859' offset 1 limit 1

# Resources

https://docs.couchbase.com/dotnet-sdk/current/hello-world/overview.html
https://docs.couchbase.com/dotnet-sdk/current/hello-world/start-using-sdk.html
https://github.com/couchbaselabs

https://medium.com/@punskaari/how-to-get-started-with-couchbase-with-asp-net-core-using-linq2couchbase-and-dependency-injection-e27b794d8126

https://github.com/iSatishYadav/getting-started-with-couchbase-linq2couchbase-aspnetcore-dependency-injection

