-- MySQL Script generated by MySQL Workbench
-- Thu May 21 19:56:23 2020
-- Model: New Model    Version: 1.0
-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema mydb
-- -----------------------------------------------------
-- -----------------------------------------------------
-- Schema votemyst
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema votemyst
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `votemyst` DEFAULT CHARACTER SET utf8 ;
USE `votemyst` ;

-- -----------------------------------------------------
-- Table `votemyst`.`authorizations`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `votemyst`.`authorizations` (
  `auth_id` INT NOT NULL AUTO_INCREMENT,
  `user_id` INT NOT NULL,
  `service_id` VARCHAR(64) NOT NULL,
  `service` BIT(8) NOT NULL,
  `valid` TINYINT NOT NULL DEFAULT '1',
  PRIMARY KEY (`auth_id`, `user_id`),
  UNIQUE INDEX `auth_id_UNIQUE` (`auth_id` ASC) VISIBLE)
ENGINE = InnoDB
AUTO_INCREMENT = 5
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `votemyst`.`entries`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `votemyst`.`entries` (
  `entry_id` INT NOT NULL AUTO_INCREMENT,
  `display_id` VARCHAR(32) NOT NULL,
  `event_id` INT NOT NULL,
  `user_id` INT NOT NULL,
  `submit_date` DATETIME NOT NULL,
  `entry_type` INT NOT NULL,
  `entry` TEXT NOT NULL,
  PRIMARY KEY (`entry_id`),
  UNIQUE INDEX `entry_id_UNIQUE` (`entry_id` ASC) VISIBLE,
  UNIQUE INDEX `display_id_UNIQUE` (`display_id` ASC) VISIBLE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `votemyst`.`events`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `votemyst`.`events` (
  `event_id` INT NOT NULL AUTO_INCREMENT,
  `display_id` VARCHAR(32) NOT NULL,
  `name` VARCHAR(64) NOT NULL,
  `url` VARCHAR(32) NULL DEFAULT NULL,
  `description` VARCHAR(512) NULL DEFAULT NULL,
  `event_type` INT NOT NULL,
  `reveal_date` DATETIME NOT NULL,
  `start_date` DATETIME NOT NULL,
  `end_date` DATETIME NOT NULL,
  `vote_end_date` DATETIME NOT NULL,
  PRIMARY KEY (`event_id`),
  UNIQUE INDEX `event_id_UNIQUE` (`event_id` ASC) VISIBLE,
  UNIQUE INDEX `display_id_UNIQUE` (`display_id` ASC) VISIBLE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `votemyst`.`reports`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `votemyst`.`reports` (
  `report_id` INT NOT NULL AUTO_INCREMENT,
  `entry_id` INT NOT NULL,
  `reason` TEXT NULL DEFAULT NULL,
  PRIMARY KEY (`report_id`),
  UNIQUE INDEX `report_id_UNIQUE` (`report_id` ASC) VISIBLE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_bin;


-- -----------------------------------------------------
-- Table `votemyst`.`useraccount`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `votemyst`.`useraccount` (
  `user_id` INT NOT NULL AUTO_INCREMENT,
  `display_id` VARCHAR(32) NOT NULL,
  `username` VARCHAR(32) NOT NULL,
  `first_seen` DATETIME NULL DEFAULT NULL,
  `global_permissions` BIGINT(20) UNSIGNED ZEROFILL NOT NULL,
  `account_badge` BIGINT(20) UNSIGNED ZEROFILL NOT NULL,
  PRIMARY KEY (`user_id`, `display_id`),
  UNIQUE INDEX `snowflake_UNIQUE` (`user_id` ASC) VISIBLE,
  UNIQUE INDEX `display_id_UNIQUE` (`display_id` ASC) VISIBLE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `votemyst`.`votes`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `votemyst`.`votes` (
  `vote_id` INT NOT NULL AUTO_INCREMENT,
  `user_id` INT NOT NULL,
  `entry_id` INT NOT NULL,
  `vote_date` DATETIME NOT NULL,
  PRIMARY KEY (`vote_id`, `user_id`, `entry_id`),
  UNIQUE INDEX `vote_id_UNIQUE` (`vote_id` ASC) VISIBLE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
