-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema database_odontologia
-- -----------------------------------------------------
DROP SCHEMA IF EXISTS `database_odontologia` ;

-- -----------------------------------------------------
-- Schema database_odontologia
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `database_odontologia` ;
USE `database_odontologia` ;

-- -----------------------------------------------------
-- Table `database_odontologia`.`usuarios`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `database_odontologia`.`usuarios` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `nome` VARCHAR(45) UNIQUE NOT NULL,
  `template` TEXT NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `template_UNIQUE` (`template` ASC))
ENGINE = InnoDB;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
