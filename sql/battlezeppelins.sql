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
  `name` VARCHAR(45) NOT NULL,
  `lastSeen` DATETIME NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `name_UNIQUE` (`name` ASC))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `battlezeppelins`.`Game`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `battlezeppelins`.`Game` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `challenger` INT UNSIGNED NOT NULL,
  `challengee` INT UNSIGNED NOT NULL,
  `gameState` INT UNSIGNED NOT NULL,
  `challengerTable` VARCHAR(5000) NOT NULL,
  `challengeeTable` VARCHAR(5000) NOT NULL,
  `challengerTurn` TINYINT(1) NULL,
  `lastOpen` VARCHAR(50) NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_player_id_1_idx` (`challenger` ASC),
  INDEX `fk_player_id_2_idx` (`challengee` ASC),
  CONSTRAINT `fk_player_id_game_1`
    FOREIGN KEY (`challenger`)
    REFERENCES `battlezeppelins`.`Player` (`id`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE,
  CONSTRAINT `fk_player_id_game_2`
    FOREIGN KEY (`challengee`)
    REFERENCES `battlezeppelins`.`Player` (`id`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `battlezeppelins`.`GameChallenge`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `battlezeppelins`.`GameChallenge` (
  `challenger` INT UNSIGNED NOT NULL,
  `challengee` INT UNSIGNED NOT NULL,
  PRIMARY KEY (`challenger`),
  INDEX `fk_player_id_2_idx` (`challengee` ASC),
  UNIQUE INDEX `challenger_UNIQUE` (`challenger` ASC),
  UNIQUE INDEX `challengee_UNIQUE` (`challengee` ASC),
  CONSTRAINT `fk_player_id_game_challenge_1`
    FOREIGN KEY (`challenger`)
    REFERENCES `battlezeppelins`.`Player` (`id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `fk_player_id_game_challenge_2`
    FOREIGN KEY (`challengee`)
    REFERENCES `battlezeppelins`.`Player` (`id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `battlezeppelins`.`Message`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `battlezeppelins`.`Message` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `player` INT UNSIGNED NOT NULL,
  `time` DATETIME NOT NULL,
  `text` VARCHAR(4000) NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_player_id_message_idx` (`player` ASC),
  CONSTRAINT `fk_player_id_message`
    FOREIGN KEY (`player`)
    REFERENCES `battlezeppelins`.`Player` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
