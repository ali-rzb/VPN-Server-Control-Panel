# Windows VPN Server Management Portal

Welcome to the Windows VPN Server Management Portal, a feature-rich C# MVC project designed to streamline the administration of your VPN server. Once installed on your server's IIS, this web-based control panel provides convenient access to a range of essential functionalities.

## Features

1. **Multilink Capacity Control:**
   - Set and manage limits on the number of connections each user can establish, optimizing server performance.

2. **IP Reservation:**
   - Assign unique private IP addresses to individual VPN users, enhancing network management and security.

3. **User Management:**
   - Effortlessly edit or update VPN user profiles through an intuitive interface.

4. **Activity Monitoring:**
   - Observe the time and duration of connections for each VPN user, gaining insights into user activity.

5. **Connection History Visualization:**
   - View user-friendly plots depicting connection trends per hour over the past few days.

6. **File Uploads:**
   - Conveniently upload files to the server directly through the portal for seamless data transfer.

7. **Firewall Control:**
   - Enable or disable the firewall for remote connections with ease, ensuring tailored network security.

8. **Server Performance Monitoring:**
   - Real-time monitoring of CPU, Memory, and Network performance for a comprehensive view of server health.

9. **Usage Statistics:**
   - Monitor and track total hours, downloads, and uploads for each user, enabling informed decision-making and resource optimization.

# Windows VPN Server Management Portal - Installation Guide

Follow these steps to set up the Windows VPN Server Management Portal on your server:

## 1. Set Up Website or Application

- Create a new Website or Application.
- [Download](https://github.com/ali-rzb/VPN-Server-Control-Panel/releases/download/main/CorpServer.rar) the compiled files of the main Web Application (Corp Server)
- Extract the files to the root of the web application.

## 2. Configure Settings Folder

- Create a folder named "ServerSettings" in the root directory of drive "C."

## 3. Configure App Pool Administrator

- Open IIS Manager.
- Navigate to Application Pools.
- Select the pool used by your web app (found in the Advanced Settings of your application).
- Right-click on the pool and choose Advanced Settings.
- Edit the Identity by selecting custom and entering an administrator username and password. Click OK.

## 4. Create User Groups

- Create "VPN Users" and "Denied Users" groups.
- In the Network Policy Server, set "VPN Users" as permitted users and "Denied Users" as forbidden.

## 5. Rename Network Adapter

- Rename the internet network adapter to "public" (the one with a public IP address) for online performance monitoring.

## 6. Add Firewall Rules

- Add firewall rules for "RDP TCP" and "RDP UDP" to enable or disable remote desktop. Modify port settings if necessary.

## 7. Enable Logging in RRAS

- Open the Routing and Remote Access panel.
- Right-click on the server, go to Properties.
- In the Logging tab, check "Log all events."

## 8. Update NPS Policy

- In the NPS panel, ensure that the policy allowing users to connect (VPN Users) has the "ignore user account dial-in properties" checkbox unchecked. This enables IP reservation.

## 9. Copy Prevent Multilink Files

- [Download](https://github.com/ali-rzb/VPN-Server-Control-Panel/releases/download/main/PreventMultilink.rar) the compiled files for the PreventMultilink project.
- Extract the "PreventMultilink" folder (the folder itself) into the "ServerSettings" folder.

## 10. Set Up Task Scheduler

- [Download](https://github.com/ali-rzb/VPN-Server-Control-Panel/releases/download/main/PreventMultilink_Tasks.zip) the exported tasks Multilink_Limit_Disconnection.xml and Multilink_Limit_Connection.xml.
- Open Task Scheduler on the server.
- Import the tasks using "Import Task" and select the respective XML files.

## 11. Set the server's IP address in settings

- Upon the initial webpage access and after logging in with an admin user, you will be prompted to provide the server's IP address.
- Enter the IP address of the server where the web application is hosted.

Your Windows VPN Server Management Portal is now ready for use. Access it through your web browser and enjoy streamlined server management.
