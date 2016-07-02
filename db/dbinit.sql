DROP DATABASE IF EXISTS `room_manager`;

CREATE DATABASE `room_manager`;

USE `room_manager`;

CREATE TABLE `rooms` (
    `id` INT(10) PRIMARY KEY AUTO_INCREMENT,
    `name` VARCHAR(30) NOT NULL UNIQUE,         
    `number` INT(10) NOT NULL,
    `type` INT(10),
    `custom_price` FLOAT DEFAULT 0,
    `capacity` INT,
    `status` INT,
    `description` TEXT,
    CONSTRAINT FOREIGN KEY room_type_fk(type) REFERENCES room_type(id) ON DELETE SET NULL
) CHARSET=UTF8;

# Room status:
# 0 - Vacant
# 1 - Reserved
# 2 - Checked-in
# 3 - In-maintenance

CREATE TABLE `room_type` (
    `id` INT(10) PRIMARY KEY AUTO_INCREMENT,
    `name` VARCHAR(30) NOT NULL,
    `typical_price` FLOAT NOT NULL
) CHARSET=UTF8;

CREATE TABLE `users` (
    `id` INT(10) PRIMARY KEY AUTO_INCREMENT,
    `username` VARCHAR(32) NOT NULL,
    `password` CHAR(32) NOT NULL,
    `privilege` INT DEFAULT 1
) CHARSET=UTF8;

# Privileges:
# 0 - Blocked
# 1 - Staff
# 2 - Administrator

CREATE TABLE `customers` (
    `id` INT(10) PRIMARY KEY AUTO_INCREMENT,
    `name` VARCHAR(32) NOT NULL,
    `gender` BOOLEAN NOT NULL,
    `identity` VARCHAR(18) NOT NULL,
    `reserve_date` REAL,
    `entry_date` REAL,
    `checkout_date` REAL,
    `status` INT NOT NULL,
    `room_id` INT(10) NOT NULL
) CHARSET=UTF8;

# Customer Status:
# 0 - In Reservation
# 1 - Checked in
# 2 - Checked out

CREATE TABLE `groups` (
    `id` INT(10) PRIMARY KEY AUTO_INCREMENT,
    `leader_id` INT(10) NOT NULL,
    `members` TEXT NOT NULL,
    `reserve_date` REAL,
    `entry_date` REAL,
    `checkout_date` REAL,
    `status` INT NOT NULL
) CHARSET=UTF8;

CREATE TABLE `consumptions` (
    `id` INT(10) PRIMARY KEY AUTO_INCREMENT,
    `customer` INT(10) NOT NULL,
    `item` INT(10) NOT NULL,
    `count` INT DEFAULT 1,
    `price` FLOAT DEFAULT 0,
    `comment` TEXT
) CHARSET=UTF8;

CREATE TABLE `items` (
    `id` INT(10) PRIMARY KEY AUTO_INCREMENT,
    `name` VARCHAR(50) NOT NULL,
    `typical_price` FLOAT NOT NULL,
    `description` TEXT
) CHARSET=UTF8;