# Windows VPN Server Management Portal

Welcome to the Windows VPN Server Management Portal, a feature-rich C# MVC project designed to streamline the administration of your VPN server. Once installed on your server's IIS, this web-based control panel provides convenient access to a range of essential functionalities.

# Screenshots

![Screenshot 2023-11-15 020937](https://github.com/ali-rzb/VPN-Server-Control-Panel/assets/63366614/6c1b6384-272a-44ca-821b-5ce591447250)

## Features

1. **Multilink Capacity Control:**

   The RRAS service on Windows Server lacks native options for controlling the number of simultaneous connections for VPN users. To fill this gap, our custom control panel introduces Multilink Capacity Control, enabling administrators to set and manage simultaneous connection limits for each user.

   
![Multilink](https://github.com/ali-rzb/VPN-Server-Control-Panel/assets/63366614/9238a75f-d354-4a66-aae7-b6f79e34a998)


3. **IP Reservation:**
   - Assign unique private IP addresses to individual VPN users, enhancing network management and security.

4. **User Management:**
   - Effortlessly edit or update VPN user profiles through an intuitive interface.

5. **Activity Monitoring:**
   - Observe the time and duration of connections for each VPN user, gaining insights into user activity.

![Screenshot 2023-11-15 094429](https://github.com/ali-rzb/VPN-Server-Control-Panel/assets/63366614/a86770cf-d75f-4ae9-b180-ea5499ff3162)

5. **Connection History Visualization:**
   - View user-friendly plots depicting connection trends per hour over the past few days.
  
![Screenshot 2023-11-15 094623](https://github.com/ali-rzb/VPN-Server-Control-Panel/assets/63366614/b5d0aa93-3bd6-48a4-a417-bd6bf900783e)


6. **File Uploads:**
   - Conveniently upload files to the server directly through the portal for seamless data transfer.

7. **Firewall Control:**
   - Enable or disable the firewall for remote connections with ease, ensuring tailored network security.

8. **Server Performance Monitoring:**
   - Real-time monitoring of CPU, Memory, and Network performance for a comprehensive view of server health.
  

![Screenshot 2023-11-19 at 10-07-14 Server Maneger - VPN Server](https://github.com/ali-rzb/VPN-Server-Control-Panel/assets/63366614/762c7eb0-34b8-4d47-96f7-dbd00ab9754c)



9. **Usage Statistics:**
   - Monitor and track total hours, downloads, and uploads for each user, enabling informed decision-making and resource optimization.
  
11. **Seperate Panel for Users:**
      - The client panel provides users with the following capabilities:
         - View their connection history and other statistical data.
         - Modify their password and personal information.
         - Access downloadable applications and instructional videos for guidance.
    

![Screenshot 2023-11-17 at 09-47-48 ali r - Haviro Server](https://github.com/ali-rzb/VPN-Server-Control-Panel/assets/63366614/64ee123f-f97e-46bd-ba6d-2639b2937749)


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

## 12. Setup Client Panel
- Create a new website or application specifically for the client panel.
- Similar to cPanel, the user's panel requires admin privileges. Adjust its user app pool to match the cPanel app pool for seamless functionality.
- [Download](https://github.com/ali-rzb/VPN-Server-Control-Panel/releases/download/main/Client.rar) the compiled files and copy them to the root directory of the client panel application.

## Note: Recaptcha Integration
Please be aware that the initial release included Google reCAPTCHA in both the cPanel and the user's panel login for enhanced security. However, due to the necessity of changing server and client-side keys, this feature was temporarily disabled in the released version.
To enable reCAPTCHA, follow these steps within your project and recompile:
   - BL.RobotValidate (Update the Keys dictionary with your keys)
   - Client.Home.Login (view) (Uncomment comments and replace your site key)
   - Client.Home.Login (controller) (Uncomment comments in the Login method)
   - CorpServer.User.Login (view) (Uncomment the commented line and replace your site key)
   - CorpServer.User.Login (controller) (Uncomment comments in the Login method)
Make these adjustments directly in your project files and then recompile. This will reactivate the reCAPTCHA feature in both the cPanel and the user's panel login, providing an added layer of security.

### Your Windows VPN Server Management Portal is now ready for use. Access it through your web browser and enjoy streamlined server management.
