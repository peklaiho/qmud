-- phpMyAdmin SQL Dump
-- version 4.5.4.1deb2ubuntu2
-- http://www.phpmyadmin.net
--
-- Host: localhost
-- Generation Time: Aug 30, 2016 at 08:01 PM
-- Server version: 10.0.25-MariaDB-0ubuntu0.16.04.1
-- PHP Version: 7.0.8-0ubuntu0.16.04.2

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `gauntlet`
--

-- --------------------------------------------------------

--
-- Table structure for table `items`
--

CREATE TABLE `items` (
  `Id` int(11) NOT NULL,
  `Article` int(11) NOT NULL COMMENT '0=None, 1=A, 2=An, 3=The',
  `Name` varchar(64) NOT NULL,
  `Keywords` varchar(64) NOT NULL,
  `ShortDesc` varchar(256) NOT NULL,
  `LongDesc` varchar(1024) NOT NULL DEFAULT ''
) ENGINE=InnoDB DEFAULT CHARSET=ascii;

-- --------------------------------------------------------

--
-- Table structure for table `monsters`
--

CREATE TABLE `monsters` (
  `Id` int(11) NOT NULL,
  `Class` int(11) NOT NULL COMMENT '0=Animal, 1=Human',
  `Flags` varchar(64) NOT NULL,
  `Name` varchar(64) NOT NULL,
  `Sex` int(11) NOT NULL COMMENT '0=Neutral, 1=Male, 2=Female',
  `Keywords` varchar(64) NOT NULL,
  `ShortDesc` varchar(256) NOT NULL,
  `LongDesc` varchar(1024) NOT NULL DEFAULT ''
) ENGINE=InnoDB DEFAULT CHARSET=ascii;

--
-- Dumping data for table `monsters`
--

INSERT INTO `monsters` (`Id`, `Class`, `Flags`, `Name`, `Sex`, `Keywords`, `ShortDesc`, `LongDesc`) VALUES
(100, 1, 'ab', 'sentry of the Black Order', 1, 'sentry,black,order', 'A sentry dressed in black armor is here, guarding the area.', '(add description)'),
(101, 1, 'ab', 'archer of the Black Order', 1, 'archer,black,order', 'An archer is here, dressed head-to-toe in black.', '(add description)'),
(102, 1, 'a', 'high priest of the Black Order', 1, 'high,priest,black,order', 'A priest of some kind is here, wearing a black robe.', '(add description)'),
(103, 0, '', 'swamp snake', 0, 'swamp,snake', 'A large swamp snake slithers on the ground.', '(add description)'),
(104, 0, '', 'green frog', 0, 'green,frog', 'A frog with green back hops around the muddy ground.', ''),
(105, 0, '', 'black crow', 0, 'black,crow', 'A black crow flies through the air, releasing loud caws.', ''),
(200, 1, 'ab', 'highwayman', 1, 'highwayman,bandit', 'A highwayman is here, stalking travelers from a bush.', '(add description)'),
(201, 1, 'b', 'guard of Greenhaven', 1, 'guard,greenhaven', 'A guard is here, dressed in exquisite green armor.', '(add description)'),
(202, 1, 'b', 'ranger of Greenhaven', 1, 'ranger,greenhaven', 'A ranger is here, camouflaged against the green surroundings.', '(add description)'),
(203, 0, '', 'squirrel', 0, 'squirrel', 'A squirrel is here, scurrying up the trunk of a large tree.', ''),
(204, 0, '', 'nightingale', 0, 'bird,nightingale', 'A nightingale is sitting on a branch, singing a beautiful tune.', ''),
(205, 0, '', 'deer', 0, 'deer', 'A deer is grazing among the lush green grass.', '');

-- --------------------------------------------------------

--
-- Table structure for table `players`
--

CREATE TABLE `players` (
  `Id` int(11) NOT NULL,
  `Name` varchar(64) NOT NULL,
  `Sex` int(11) NOT NULL,
  `Level` int(11) NOT NULL,
  `Password` varchar(64) NOT NULL,
  `Preferences` varchar(64) NOT NULL,
  `ColorPrefs` varchar(256) NOT NULL,
  `LineLength` int(11) NOT NULL DEFAULT '80',
  `UpdatedAt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `CreatedAt` date NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=ascii;

-- --------------------------------------------------------

--
-- Table structure for table `player_items`
--

CREATE TABLE `player_items` (
  `PlayerId` int(11) NOT NULL,
  `Number` int(11) NOT NULL,
  `ItemId` int(11) NOT NULL,
  `Parent` int(11) NOT NULL DEFAULT '0',
  `Location` int(11) NOT NULL DEFAULT '0' COMMENT '0=inv, 1=eq'
) ENGINE=InnoDB DEFAULT CHARSET=ascii;

-- --------------------------------------------------------

--
-- Table structure for table `zones`
--

CREATE TABLE `zones` (
  `Id` int(11) NOT NULL,
  `Name` varchar(64) COLLATE utf8_unicode_ci NOT NULL,
  `Width` int(11) NOT NULL,
  `Height` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Dumping data for table `zones`
--

INSERT INTO `zones` (`Id`, `Name`, `Width`, `Height`) VALUES
(1, 'Swamps of the Blackmarsh', 10, 10),
(2, 'Fields of Greenhaven', 10, 10);

-- --------------------------------------------------------

--
-- Table structure for table `zone_monsters`
--

CREATE TABLE `zone_monsters` (
  `ZoneId` int(11) NOT NULL,
  `MonsterId` int(11) NOT NULL,
  `Count` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Dumping data for table `zone_monsters`
--

INSERT INTO `zone_monsters` (`ZoneId`, `MonsterId`, `Count`) VALUES
(1, 100, 15),
(1, 101, 15),
(1, 102, 10),
(1, 103, 15),
(1, 104, 15),
(1, 105, 15),
(2, 200, 10),
(2, 201, 15),
(2, 202, 15),
(2, 203, 15),
(2, 204, 15),
(2, 205, 15);

--
-- Indexes for dumped tables
--

--
-- Indexes for table `items`
--
ALTER TABLE `items`
  ADD PRIMARY KEY (`Id`);

--
-- Indexes for table `monsters`
--
ALTER TABLE `monsters`
  ADD PRIMARY KEY (`Id`);

--
-- Indexes for table `players`
--
ALTER TABLE `players`
  ADD PRIMARY KEY (`Id`);

--
-- Indexes for table `player_items`
--
ALTER TABLE `player_items`
  ADD PRIMARY KEY (`PlayerId`,`Number`);

--
-- Indexes for table `zones`
--
ALTER TABLE `zones`
  ADD PRIMARY KEY (`Id`);

--
-- Indexes for table `zone_monsters`
--
ALTER TABLE `zone_monsters`
  ADD PRIMARY KEY (`ZoneId`,`MonsterId`);

--
-- AUTO_INCREMENT for dumped tables
--

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
