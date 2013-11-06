SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='TRADITIONAL,ALLOW_INVALID_DATES';

DROP SCHEMA IF EXISTS `battlezeppelins` ;
CREATE SCHEMA IF NOT EXISTS `battlezeppelins` DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci ;
USE `battlezeppelins` ;

-- -----------------------------------------------------
-- Table `battlezeppelins`.`Player`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `battlezeppelins`.`Player` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(45) NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `name_UNIQUE` (`name` ASC))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `battlezeppelins`.`Game`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `battlezeppelins`.`Game` (
  `id` INT NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`id`))
ENGINE = InnoDB;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
