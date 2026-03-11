-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Mar 11, 2026 at 06:47 AM
-- Server version: 10.4.32-MariaDB
-- PHP Version: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `stadium_demo`
--

DELIMITER $$
--
-- Procedures
--
CREATE DEFINER=`root`@`localhost` PROCEDURE `delete_match` (IN `mid` INT)   BEGIN
    DELETE FROM ticket  WHERE match_id = mid;
    DELETE FROM matches WHERE match_id = mid;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `delete_user` (IN `uid` INT)   BEGIN
    DELETE FROM ticket WHERE user_id = uid;
    DELETE FROM user   WHERE user_id = uid;
END$$

DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `matches`
--

CREATE TABLE `matches` (
  `match_id` int(11) NOT NULL,
  `match_name` varchar(100) NOT NULL,
  `series_tournament_name` varchar(120) NOT NULL,
  `match_format` varchar(10) NOT NULL,
  `match_date` varchar(20) NOT NULL,
  `match_time` varchar(20) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `matches`
--

INSERT INTO `matches` (`match_id`, `match_name`, `series_tournament_name`, `match_format`, `match_date`, `match_time`) VALUES
(101, 'IND vs PAK', 'ICC T20 World Cup 2025', 'T20', '15/06/2026', '14:00:00'),
(102, 'IND vs PAK', 'Asia Cup 2025', 'ODI', '22/08/2026', '13:30:00'),
(103, 'IND vs PAK', 'ICC Champions Trophy 2025', 'ODI', '21/03/2026', '14:00:00'),
(104, 'IND vs AUS', 'Border-Gavaskar Trophy 2025', 'Test', '10/04/2026', '09:30:00'),
(105, 'IND vs AUS', 'Border-Gavaskar Trophy 2025', 'Test', '18/05/2026', '09:30:00'),
(106, 'IND vs AUS', 'India vs Australia T20 Series', 'T20', '25/11/2026', '19:00:00'),
(107, 'IND vs AUS', 'India vs Australia T20 Series', 'T20', '28/11/2026', '19:00:00'),
(108, 'IND vs AUS', 'India vs Australia ODI Series', 'ODI', '05/12/2026', '13:30:00'),
(109, 'IND vs AUS', 'India vs Australia ODI Series', 'ODI', '09/12/2026', '13:30:00'),
(110, 'IND vs ENG', 'India vs England Test Series', 'Test', '25/01/2026', '09:30:00'),
(111, 'IND vs ENG', 'India vs England Test Series', 'Test', '05/02/2026', '09:30:00'),
(112, 'IND vs ENG', 'India vs England ODI Series', 'ODI', '15/09/2026', '13:30:00'),
(113, 'IND vs ENG', 'India vs England ODI Series', 'ODI', '18/09/2026', '13:30:00'),
(114, 'IND vs ENG', 'ICC T20 World Cup 2025', 'T20', '18/06/2026', '14:00:00'),
(115, 'IND vs SA', 'India vs South Africa T20 Series', 'T20', '10/10/2026', '19:00:00'),
(116, 'IND vs SA', 'India vs South Africa T20 Series', 'T20', '13/10/2026', '19:00:00'),
(117, 'IND vs SA', 'India vs South Africa T20 Series', 'T20', '16/10/2026', '19:00:00'),
(118, 'IND vs SA', 'India vs South Africa ODI Series', 'ODI', '05/10/2026', '13:30:00'),
(119, 'IND vs SA', 'India vs South Africa ODI Series', 'ODI', '08/10/2026', '13:30:00'),
(120, 'IND vs SA', 'ICC T20 World Cup 2025 — SF', 'T20', '26/06/2026', '14:00:00'),
(121, 'IND vs NZ', 'India vs New Zealand Test Series', 'Test', '16/10/2026', '09:30:00'),
(122, 'IND vs NZ', 'India vs New Zealand Test Series', 'Test', '24/10/2026', '09:30:00'),
(123, 'IND vs NZ', 'India vs New Zealand Test Series', 'Test', '01/11/2026', '09:30:00'),
(124, 'IND vs NZ', 'India vs New Zealand T20 Series', 'T20', '20/04/2026', '19:00:00'),
(125, 'IND vs NZ', 'India vs New Zealand T20 Series', 'T20', '23/10/2026', '19:00:00'),
(126, 'IND vs SL', 'Asia Cup 2025', 'ODI', '28/08/2026', '13:30:00'),
(127, 'IND vs SL', 'India vs Sri Lanka T20 Series', 'T20', '03/08/2026', '19:00:00'),
(128, 'IND vs SL', 'India vs Sri Lanka T20 Series', 'T20', '06/08/2026', '19:00:00'),
(129, 'IND vs SL', 'India vs Sri Lanka ODI Series', 'ODI', '10/08/2026', '13:30:00'),
(130, 'IND vs WI', 'India vs West Indies ODI Series', 'ODI', '02/02/2026', '13:30:00'),
(131, 'IND vs WI', 'India vs West Indies ODI Series', 'ODI', '06/02/2026', '13:30:00'),
(132, 'IND vs WI', 'India vs West Indies T20 Series', 'T20', '10/02/2026', '19:00:00'),
(133, 'IND vs WI', 'India vs West Indies T20 Series', 'T20', '13/02/2026', '19:00:00'),
(134, 'IND vs BAN', 'Asia Cup 2025', 'ODI', '25/08/2026', '13:30:00'),
(135, 'IND vs BAN', 'India vs Bangladesh T20 Series', 'T20', '12/10/2026', '19:00:00'),
(136, 'IND vs BAN', 'India vs Bangladesh T20 Series', 'T20', '15/10/2026', '19:00:00'),
(137, 'IND vs BAN', 'India vs Bangladesh Test Series', 'Test', '20/11/2026', '09:30:00'),
(138, 'IND vs AFG', 'ICC T20 World Cup 2025', 'T20', '20/06/2026', '14:00:00'),
(139, 'IND vs AFG', 'India vs Afghanistan T20 Series', 'T20', '15/01/2026', '19:00:00'),
(140, 'AUS vs ENG', 'The Ashes 2025', 'Test', '05/07/2026', '10:00:00'),
(141, 'AUS vs ENG', 'The Ashes 2025', 'Test', '17/07/2026', '10:00:00'),
(142, 'AUS vs ENG', 'The Ashes 2025', 'Test', '31/07/2026', '10:00:00'),
(143, 'AUS vs ENG', 'ICC T20 World Cup 2025', 'T20', '14/06/2026', '14:00:00'),
(144, 'AUS vs ENG', 'AUS vs ENG ODI Series', 'ODI', '20/07/2026', '13:30:00'),
(145, 'PAK vs ENG', 'Pakistan vs England Test Series', 'Test', '12/11/2026', '09:30:00'),
(146, 'PAK vs ENG', 'ICC Champions Trophy 2025', 'ODI', '04/03/2026', '14:00:00'),
(147, 'PAK vs ENG', 'Pakistan vs England T20 Series', 'T20', '22/11/2026', '19:00:00'),
(148, 'PAK vs AUS', 'Pakistan vs Australia ODI Series', 'ODI', '15/03/2026', '13:30:00'),
(149, 'PAK vs AUS', 'Pakistan vs Australia T20 Series', 'T20', '20/03/2026', '19:00:00'),
(150, 'AUS vs PAK', 'ICC T20 World Cup 2025 — SF', 'T20', '27/06/2026', '14:00:00'),
(151, 'SA vs AUS', 'SA vs Australia T20 Series', 'T20', '28/09/2026', '19:00:00'),
(152, 'SA vs AUS', 'SA vs Australia ODI Series', 'ODI', '02/10/2026', '13:30:00'),
(153, 'NZ vs ENG', 'New Zealand vs England ODI Series', 'ODI', '18/02/2026', '13:30:00'),
(154, 'NZ vs ENG', 'New Zealand vs England T20 Series', 'T20', '22/02/2026', '19:00:00'),
(155, 'SL vs PAK', 'Asia Cup 2025', 'ODI', '31/08/2026', '13:30:00'),
(156, 'SL vs PAK', 'Sri Lanka vs Pakistan T20 Series', 'T20', '05/09/2026', '19:00:00'),
(157, 'SA vs ENG', 'SA vs England T20 Series', 'T20', '12/12/2026', '19:00:00'),
(158, 'SA vs ENG', 'SA vs England ODI Series', 'ODI', '16/12/2026', '13:30:00'),
(159, 'IND vs AUS', 'ICC T20 World Cup 2025 — FINAL', 'T20', '29/06/2026', '14:00:00'),
(160, 'IND vs NZ', 'ICC Champions Trophy 2025', 'ODI', '06/03/2026', '14:00:00'),
(161, 'AUS vs SA', 'ICC Champions Trophy 2025', 'ODI', '07/03/2026', '14:00:00'),
(162, 'ENG vs BAN', 'ICC Champions Trophy 2025', 'ODI', '08/03/2026', '14:00:00'),
(163, 'IND vs SA', 'ICC Champions Trophy 2025 — SF', 'ODI', '11/03/2026', '14:00:00'),
(164, 'IND vs AUS', 'ICC Champions Trophy 2025 — Final', 'ODI', '14/03/2026', '14:00:00'),
(165, 'IND vs SL', 'Asia Cup 2025 — Final', 'ODI', '07/09/2026', '14:00:00'),
(201, 'CSK vs MI', 'IPL 2025', 'T20', '22/03/2026', '19:30:00'),
(202, 'RCB vs KKR', 'IPL 2025', 'T20', '23/03/2026', '19:30:00'),
(203, 'SRH vs RR', 'IPL 2025', 'T20', '25/03/2026', '19:30:00'),
(204, 'DC vs PBKS', 'IPL 2025', 'T20', '26/03/2026', '19:30:00'),
(205, 'GT vs LSG', 'IPL 2025', 'T20', '27/03/2026', '19:30:00'),
(206, 'MI vs RCB', 'IPL 2025', 'T20', '30/03/2026', '19:30:00'),
(207, 'CSK vs SRH', 'IPL 2025', 'T20', '31/03/2026', '19:30:00'),
(208, 'KKR vs DC', 'IPL 2025', 'T20', '01/04/2026', '19:30:00'),
(209, 'RR vs GT', 'IPL 2025', 'T20', '02/04/2026', '19:30:00'),
(210, 'PBKS vs LSG', 'IPL 2025', 'T20', '04/04/2026', '19:30:00'),
(211, 'MI vs CSK', 'IPL 2025', 'T20', '06/04/2026', '15:30:00'),
(212, 'RCB vs SRH', 'IPL 2025', 'T20', '07/04/2026', '19:30:00'),
(213, 'KKR vs RR', 'IPL 2025', 'T20', '08/04/2026', '19:30:00'),
(214, 'GT vs DC', 'IPL 2025', 'T20', '09/04/2026', '19:30:00'),
(215, 'LSG vs CSK', 'IPL 2025', 'T20', '11/04/2026', '19:30:00'),
(216, 'MI vs KKR', 'IPL 2025', 'T20', '13/04/2026', '19:30:00'),
(217, 'RR vs PBKS', 'IPL 2025', 'T20', '14/04/2026', '19:30:00'),
(218, 'SRH vs GT', 'IPL 2025', 'T20', '15/04/2026', '19:30:00'),
(219, 'DC vs RCB', 'IPL 2025', 'T20', '16/04/2026', '19:30:00'),
(220, 'CSK vs KKR', 'IPL 2025', 'T20', '18/04/2026', '19:30:00'),
(221, 'MI vs GT', 'IPL 2025', 'T20', '20/04/2026', '15:30:00'),
(222, 'LSG vs RR', 'IPL 2025', 'T20', '21/04/2026', '19:30:00'),
(223, 'PBKS vs SRH', 'IPL 2025', 'T20', '23/04/2026', '19:30:00'),
(224, 'RCB vs CSK', 'IPL 2025', 'T20', '25/04/2026', '19:30:00'),
(225, 'KKR vs MI', 'IPL 2025', 'T20', '27/04/2026', '15:30:00'),
(226, 'GT vs RR', 'IPL 2025', 'T20', '28/04/2026', '19:30:00'),
(227, 'DC vs SRH', 'IPL 2025', 'T20', '30/04/2026', '19:30:00'),
(228, 'CSK vs PBKS', 'IPL 2025', 'T20', '02/05/2026', '19:30:00'),
(229, 'MI vs LSG', 'IPL 2025', 'T20', '04/05/2026', '15:30:00'),
(230, 'RR vs RCB', 'IPL 2025', 'T20', '05/05/2026', '19:30:00'),
(231, 'KKR vs GT', 'IPL 2025', 'T20', '07/05/2026', '19:30:00'),
(232, 'SRH vs CSK', 'IPL 2025', 'T20', '09/05/2026', '19:30:00'),
(233, 'PBKS vs RCB', 'IPL 2025', 'T20', '11/05/2026', '15:30:00'),
(234, 'DC vs MI', 'IPL 2025', 'T20', '12/05/2026', '19:30:00'),
(235, 'LSG vs KKR', 'IPL 2025', 'T20', '13/05/2026', '19:30:00'),
(236, 'RR vs CSK', 'IPL 2025', 'T20', '14/05/2026', '19:30:00'),
(237, 'GT vs RCB', 'IPL 2025', 'T20', '16/05/2026', '19:30:00'),
(239, 'MI vs RR', 'IPL 2025 — Eliminator', 'T20', '21/05/2026', '19:30:00'),
(240, 'KKR vs MI', 'IPL 2025 — Qualifier 2', 'T20', '23/05/2026', '19:30:00'),
(241, 'KKR vs CSK', 'IPL 2025 — FINAL', 'T20', '25/05/2026', '19:30:00');

-- --------------------------------------------------------

--
-- Table structure for table `staff`
--

CREATE TABLE `staff` (
  `staff_id` int(11) NOT NULL,
  `password` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `staff`
--

INSERT INTO `staff` (`staff_id`, `password`) VALUES
(1001, '1234'),
(1002, 'admin@123'),
(1003, 'staff2025');

-- --------------------------------------------------------

--
-- Table structure for table `ticket`
--

CREATE TABLE `ticket` (
  `ticket_id` int(11) NOT NULL,
  `match_id` int(11) NOT NULL,
  `user_id` int(11) NOT NULL,
  `stand` char(1) NOT NULL,
  `ticket_price` int(11) NOT NULL,
  `no_of_tickets` int(11) NOT NULL,
  `total_payments` int(11) NOT NULL,
  `payment_method` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `ticket`
--

INSERT INTO `ticket` (`ticket_id`, `match_id`, `user_id`, `stand`, `ticket_price`, `no_of_tickets`, `total_payments`, `payment_method`) VALUES
(1, 101, 2001, 'D', 10000, 2, 20000, 'UPI'),
(2, 101, 2002, 'B', 7000, 3, 21000, 'Credit Card'),
(3, 101, 2003, 'A', 1000, 5, 5000, 'UPI'),
(4, 101, 2004, 'C', 5000, 2, 10000, 'Debit Card'),
(5, 101, 2005, 'D', 10000, 4, 40000, 'Netbanking'),
(6, 101, 2006, 'B', 7000, 2, 14000, 'UPI'),
(7, 103, 2007, 'D', 10000, 2, 20000, 'Credit Card'),
(8, 103, 2008, 'B', 7000, 4, 28000, 'UPI'),
(9, 103, 2009, 'A', 1000, 6, 6000, 'Debit Card'),
(10, 103, 2010, 'C', 5000, 3, 15000, 'UPI'),
(11, 104, 2011, 'A', 1000, 4, 4000, 'UPI'),
(12, 104, 2012, 'C', 5000, 2, 10000, 'Netbanking'),
(13, 104, 2013, 'B', 7000, 2, 14000, 'Credit Card'),
(14, 110, 2014, 'A', 1000, 5, 5000, 'UPI'),
(15, 110, 2015, 'B', 7000, 2, 14000, 'Debit Card'),
(16, 114, 2001, 'C', 5000, 2, 10000, 'UPI'),
(17, 114, 2016, 'D', 10000, 2, 20000, 'Credit Card'),
(18, 114, 2017, 'B', 7000, 3, 21000, 'UPI'),
(19, 120, 2018, 'D', 10000, 2, 20000, 'Netbanking'),
(20, 120, 2019, 'B', 7000, 4, 28000, 'UPI'),
(21, 120, 2020, 'C', 5000, 3, 15000, 'Credit Card'),
(22, 159, 2001, 'D', 10000, 2, 20000, 'UPI'),
(23, 159, 2002, 'D', 10000, 2, 20000, 'Credit Card'),
(24, 159, 2003, 'B', 7000, 4, 28000, 'Netbanking'),
(25, 159, 2004, 'C', 5000, 2, 10000, 'UPI'),
(26, 159, 2005, 'B', 7000, 3, 21000, 'Debit Card'),
(27, 164, 2006, 'D', 10000, 2, 20000, 'UPI'),
(28, 164, 2007, 'B', 7000, 2, 14000, 'Credit Card'),
(29, 164, 2008, 'C', 5000, 4, 20000, 'UPI'),
(30, 201, 2009, 'A', 1000, 4, 4000, 'UPI'),
(31, 201, 2010, 'B', 7000, 2, 14000, 'Debit Card'),
(32, 201, 2011, 'C', 5000, 3, 15000, 'UPI'),
(33, 201, 2012, 'D', 10000, 1, 10000, 'Credit Card'),
(34, 202, 2013, 'A', 1000, 6, 6000, 'UPI'),
(35, 202, 2014, 'B', 7000, 2, 14000, 'Netbanking'),
(36, 202, 2015, 'C', 5000, 2, 10000, 'UPI'),
(37, 241, 2001, 'D', 10000, 4, 40000, 'UPI'),
(38, 241, 2002, 'D', 10000, 2, 20000, 'Netbanking'),
(39, 241, 2003, 'B', 7000, 4, 28000, 'Credit Card'),
(40, 241, 2016, 'C', 5000, 3, 15000, 'UPI'),
(41, 241, 2017, 'B', 7000, 2, 14000, 'Debit Card'),
(42, 115, 2018, 'A', 1000, 3, 3000, 'UPI'),
(43, 115, 2019, 'B', 7000, 2, 14000, 'Credit Card'),
(44, 116, 2020, 'C', 5000, 2, 10000, 'UPI'),
(45, 121, 2009, 'A', 1000, 2, 2000, 'Debit Card'),
(46, 121, 2010, 'B', 7000, 2, 14000, 'UPI'),
(47, 130, 2011, 'A', 1000, 5, 5000, 'UPI'),
(48, 130, 2012, 'C', 5000, 2, 10000, 'Netbanking'),
(49, 134, 2013, 'A', 1000, 4, 4000, 'UPI'),
(50, 134, 2014, 'B', 7000, 2, 14000, 'Credit Card'),
(51, 164, 2001, 'A', 1000, 2, 2000, 'UPI'),
(52, 101, 1234, 'A', 1000, 8, 8000, 'UPI'),
(53, 101, 10, 'D', 10000, 10, 100000, 'Debit Card'),
(54, 201, 10, 'A', 1000, 1, 1000, 'UPI'),
(55, 108, 11, 'A', 1000, 5, 5000, 'UPI'),
(56, 137, 12, 'B', 7000, 2, 14000, 'UPI'),
(57, 107, 12, 'C', 5000, 1, 5000, 'UPI'),
(58, 237, 10, 'D', 10000, 3, 30000, 'Debit Card'),
(59, 113, 13, 'B', 7000, 1, 7000, 'UPI'),
(60, 234, 13, 'C', 5000, 3, 15000, 'UPI'),
(61, 110, 10, 'A', 1000, 9, 9000, 'UPI'),
(62, 201, 10, 'B', 7000, 5, 35000, 'UPI'),
(63, 106, 10, 'A', 1000, 5, 5000, 'UPI'),
(64, 104, 13, 'A', 1000, 1, 1000, 'UPI'),
(65, 104, 2145, 'A', 1000, 1, 1000, 'UPI'),
(66, 111, 2145, 'A', 1000, 1, 1000, 'UPI'),
(67, 112, 2145, 'D', 10000, 1, 10000, 'UPI'),
(68, 103, 2146, 'A', 1000, 1, 1000, 'UPI');

-- --------------------------------------------------------

--
-- Table structure for table `user`
--

CREATE TABLE `user` (
  `user_id` int(11) NOT NULL,
  `user_name` varchar(100) NOT NULL,
  `password` varchar(100) NOT NULL,
  `mobile_no` varchar(15) NOT NULL,
  `email` varchar(150) NOT NULL DEFAULT ''
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `user`
--

INSERT INTO `user` (`user_id`, `user_name`, `password`, `mobile_no`, `email`) VALUES
(10, 'Bhavya', 'Bhavya@1234', '6359328250', 'bhanderibhavya15@gmail.com'),
(11, 'Utsav Patel', 'Utsav@1234', '9081975773', 'tradautsav@gmail.com'),
(12, 'Dhavan Gohel', 'Dhavan@1234', '9904158677', 'dhavangohel0@gmail.com'),
(13, 'Bhavya patel', 'Bhavya@1234', '8320218050', 'bhanderibhavya15@gmail.com'),
(1234, 'Meet Panara', 'Meet@1234', '7862823053', 'panarameet78@gmail.com'),
(2001, 'Aarav Shah', 'fan@123', '9876543210', 'aarav.shah@gmail.com'),
(2002, 'Priya Patel', 'priya123', '9812345678', 'priya.patel@gmail.com'),
(2003, 'Rohan Mehta', 'rohan@456', '9988776655', 'rohan.mehta@gmail.com'),
(2004, 'Sneha Gupta', 'sneha2025', '9123456789', 'sneha.gupta@gmail.com'),
(2005, 'Vikram Singh', 'vikram@1', '9012345678', 'vikram.singh@gmail.com'),
(2006, 'Anjali Verma', 'anjali#99', '8901234567', 'anjali.verma@gmail.com'),
(2007, 'Kiran Desai', 'kiran2025', '8800112233', 'kiran.desai@gmail.com'),
(2008, 'Manish Joshi', 'manish@55', '8700223344', 'manish.joshi@gmail.com'),
(2009, 'Deepa Nair', 'deepa@nair', '8600334455', 'deepa.nair@gmail.com'),
(2010, 'Arjun Reddy', 'arjun@red', '8500445566', 'arjun.reddy@gmail.com'),
(2011, 'Pooja Sharma', 'pooja@111', '8400556677', 'pooja.sharma@gmail.com'),
(2012, 'Suresh Kumar', 'suresh@007', '8300667788', 'suresh.kumar@gmail.com'),
(2013, 'Nisha Pillai', 'nisha2025', '8200778899', 'nisha.pillai@gmail.com'),
(2014, 'Raj Malhotra', 'raj@malho', '8100889900', 'raj.malhotra@gmail.com'),
(2015, 'Kavya Iyer', 'kavya@iyer', '9900112233', 'kavya.iyer@gmail.com'),
(2016, 'Amit Dubey', 'amit@dub', '9800223344', 'amit.dubey@gmail.com'),
(2017, 'Ritu Bansal', 'ritu@ban', '9700334455', 'ritu.bansal@gmail.com'),
(2018, 'Harsh Chauhan', 'harsh@ch', '9600445566', 'harsh.chauhan@gmail.com'),
(2019, 'Divya Mishra', 'divya@m', '9500556677', 'divya.mishra@gmail.com'),
(2020, 'Sanjay Rawat', 'sanjay@r', '9400667788', 'sanjay.rawat@gmail.com'),
(2145, 'nish', 'Nish@1234', '9874563210', 'nishantsojitra8@gmail.com'),
(2146, 'Bhanderi Bhavya', 'Bhavya@1234', '1234568520', 'bhanderibhavya15@gmail.com');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `matches`
--
ALTER TABLE `matches`
  ADD PRIMARY KEY (`match_id`);

--
-- Indexes for table `staff`
--
ALTER TABLE `staff`
  ADD PRIMARY KEY (`staff_id`);

--
-- Indexes for table `ticket`
--
ALTER TABLE `ticket`
  ADD PRIMARY KEY (`ticket_id`),
  ADD KEY `match_id` (`match_id`),
  ADD KEY `user_id` (`user_id`);

--
-- Indexes for table `user`
--
ALTER TABLE `user`
  ADD PRIMARY KEY (`user_id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `ticket`
--
ALTER TABLE `ticket`
  MODIFY `ticket_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=69;

--
-- AUTO_INCREMENT for table `user`
--
ALTER TABLE `user`
  MODIFY `user_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2147;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `ticket`
--
ALTER TABLE `ticket`
  ADD CONSTRAINT `ticket_ibfk_1` FOREIGN KEY (`match_id`) REFERENCES `matches` (`match_id`),
  ADD CONSTRAINT `ticket_ibfk_2` FOREIGN KEY (`user_id`) REFERENCES `user` (`user_id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
