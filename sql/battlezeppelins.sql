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
-- Table `battlezeppelins`.`GameState`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `battlezeppelins`.`GameState` (
  `id` INT UNSIGNED NOT NULL,
  `stateName` VARCHAR(45) NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `stateName_UNIQUE` (`stateName` ASC))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `battlezeppelins`.`Game`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `battlezeppelins`.`Game` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `challenger` INT UNSIGNED NOT NULL,
  `challengee` INT UNSIGNED NOT NULL,
  `gameState` INT UNSIGNED NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_player_id_1_idx` (`challenger` ASC),
  INDEX `fk_player_id_2_idx` (`challengee` ASC),
  INDEX `fk_gamestate_id_idx` (`gameState` ASC),
  CONSTRAINT `fk_player_id_game_1`
    FOREIGN KEY (`challenger`)
    REFERENCES `battlezeppelins`.`Player` (`id`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE,
  CONSTRAINT `fk_player_id_game_2`
    FOREIGN KEY (`challengee`)
    REFERENCES `battlezeppelins`.`Player` (`id`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE,
  CONSTRAINT `fk_gamestate_id`
    FOREIGN KEY (`gameState`)
    REFERENCES `battlezeppelins`.`GameState` (`id`)
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


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;

-- -----------------------------------------------------
-- Data for table `battlezeppelins`.`GameState`
-- -----------------------------------------------------
START TRANSACTION;
USE `battlezeppelins`;
INSERT INTO `battlezeppelins`.`GameState` (`id`, `stateName`) VALUES (0, 'IN_PROGRESS');
INSERT INTO `battlezeppelins`.`GameState` (`id`, `stateName`) VALUES (1, 'CHALLENGER_WON');
INSERT INTO `battlezeppelins`.`GameState` (`id`, `stateName`) VALUES (2, 'CHALLENGEE_WON');

COMMIT;

