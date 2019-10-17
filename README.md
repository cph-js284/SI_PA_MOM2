# Systemm Integration - Programming Assignment MOM2

# What is it
This repo contains to folders, one holding the bankapp, and one holding the customerapp. Both apps are C# (dotnet core target 3.0). and are setup to rn in docker containers

# Info
When a bank instance is created using the bankapp, it automaticly randomizes its loan interest into 3 quotas: 0-99, 100-1000 and 1000 and up. Interests are set somewhere between 10.0 procent and 20.0 procent.<br>
<br>
The Exchange uses a "fan-out" to send to every bank connected and has a single-queue for received replies. Each request for loan offer is tagged with a GUID to ensure that the reponse is directed back to the propper requester(loan applicant).<br>
The timeout to reply to a loan request is set to 2500 ms.

# Requirements
Docker must be installed on your machine

# How to make it go
1) Start a container with RabbitMQ
```
sudo docker run -d --name SomeRabbit -p 5672:5672 -p 15673:15672 rabbitmq:3-management
```
NB. this container might need 10'ish sec. to spin up the rabbit inside it.<br>
<br>
In this example we will be starting 3 Banks to reply to our loan request (you can start as many as you like, remember to give them different names)
2) Start the 1st Bank container - in a new terminal
```
sudo docker run -it --link SomeRabbit --name bankone cphjs284/si_bank HighRollersBank
```
3) Start the 2nd Bank container - in a new terminal
```
sudo docker run -it --link SomeRabbit --name banktwo cphjs284/si_bank LowRollersBank
```
4) Start the 3rd Bank container - in a new terminal
```
sudo docker run -it --link SomeRabbit --name bankthree cphjs284/si_bank MaryAndJuannasBank
```
5) Start the customer container - in a new terminal 
```
sudo docker run -it --rm --link SomeRabbit --name custapp cphjs284/si_customer
```
6) Enter the amount you want to received a loanoffer and wait for the 3 banks to reply. Each reply is printed to the console (mostly for debug/review) - after all offers are received the best offer is presented in the console.

# Clean up
Removing the banks and rabbit
```
sudo docker rm -f bankone
sudo docker rm -f banktwo
sudo docker rm -f bankthree
sudo docker rm -f SomeRabbit
```