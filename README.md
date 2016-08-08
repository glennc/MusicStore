# Music Store

The intent of this fork is to break up Music Store into smaller logical pieces and deploy allow it to be deployed to a Docker Swarm Mode cluster, being able to take advantage of scalability and other features of containerisation.

You can see the journey via the tags in the repo named the same as each of the following headings:

### Stage1 (proof of concept):

This is the "proof that we can get something working" stage.

At this point in the process we have succesfully factored out a few of the queries that Music Store is making into a Web API service. However, we are just using a SQLite database that we seed on app start.

*Gains:*

	- Proved we can get something workin on a swarm mode cluster, which means the two services need to be able to talk to each other and we need to be able to hit the web front end from outside the cluster. There are some configuration steps on the cluster to make this work, but it is possible with the code at this label.
	- The deploy.sh script shows the commands that can deploy the app. It assumes a cluster is already configured.

*Obvious Problems in Stage1:*

	- Logging is currently really bad. It is possible to see the console logs of a running container, but when running on a cluster it is difficult to do that (I ended up using SSH to access each node so that I could do `docker logs <id>` on whatever node the container ended up on.)
	- Data acess is obvious a problem, we will need to decide how we want to handle that.

### Stage2 (Fix Logging):

One of our obvious problems with stage1 was logging, and we are going to fix that now without actually changing the application at all. There are many ways we could have fixed this and even more technologies and services available to help. I picked one that I liked, that worked well enough, without giving it too much though. So don't read too deeply into my selection of technology or methodology on this :).

Instead of changing the app, we are going to deploy a [logspout](https://github.com/gliderlabs/logspout) image to our cluster and instruct it to direct all logs from all containers to a [Papertrail](http://papertrailapp.com) log destination. The Logspout container will be registered as a global service, so that one container runs on every node, and because our application is already writing all of its logs to the console they will just be collected and passed on.

Steps to setup logging are:

	1. Create a Papertrail account
	2. Create a global service:
		`docker service create --mode global --mount type=bind,source=/var/run/docker.sock,target=/var/run/docker.sock  --name logger gliderlabs/logspout syslog+tls://<my papertrail URL>

Once you've done this if you do `docker service ps logger` you should see the service running on all nodes. If you now use the app all your logs will appear in the Papertrail events log and be searchable and viewable.

//TODO: Add an image of the ps command.

### Stage3 (data):

In  Stage3 we modified the Album service and MusicStore to be able to use a single SQL Server database instead of the container-local data that was being used before. In my case I am using a SQL Azure database that each container in the cluster can talk to.

*Gains:*
	- Moved data out of the containers, a container is supposed to be ephemeral and as such can only contain data that doesn't change during execution of the app. If one of our containers falls over, or a machine stops working, we need to be able to spin up a completely new one to take its place.

*Potential Problems:*
	- Shared Data between the app and the Album service, and if we follow this pattern other services we split off from the app.
		- Shared data isn't a problem per-se, but by sharing database tables there is a vector for an update in one part of the app to break another part of the app.

*Problems:*
	- Secrets: The secret we have right now is the SQL Azure connection string when deploying to a cluster. In the setup as it stands at this label I was putting the connection string in an environment variable, and even have it in the docker-compose file which is particularly bad since it is checked into source control. We need to work out the right way to deploy our app with a connection string that is not in source control and ideally secured for just our application, not other processes running on the cluster.
	- Data Migrations: We have no setup for modifying the database as we deploy changes to the app.

### Stage 4 (Secrets):


### Stage 5 (Data migrations):

