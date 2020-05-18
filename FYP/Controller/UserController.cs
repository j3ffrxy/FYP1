﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FYP.Models;
using System;
using System.Collections.Generic;
using System.Data;
namespace FYP.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            DataTable dt = DBUtl.GetTable("SELECT full_name AS [Full Name] , group_name AS Rank , dob AS [Date of Birth] FROM Users U INNER JOIN User_group UG ON U.Group_id = UG.Group_id ");
            return View("Index", dt.Rows);
        }
        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        public IActionResult Create(Users user)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Invalid Input";
                ViewData["MsgType"] = "warning";
                return View("Create");
            }
            else
            {
                string insert =
                    @"INSERT INTO Users(Group_id , full_name , dob)
                      Values ('{0}' , '{1}' , '{2:dd-MM-yyyy}')";

                int res = DBUtl.ExecSQL(insert, user.Group_id, user.full_name, user.dob);

                if (res == 1)
                {
                    TempData["Message"] = "User Created";
                    TempData["MsgType"] = "success";
                }
                else
                {
                    TempData["Message"] = DBUtl.DB_Message;
                    TempData["MsgType"] = "danger";
                }
                return RedirectToAction("Index");
            }
        }
        public IActionResult Edit(int id)
        {
            string select = "SELECT * FROM Users WHERE full_name = '{0}'";
            List<Users> list = DBUtl.GetList<Users>(select, id);
            if (list.Count == 1)
            {
                return View(list[0]);
            }
            else
            {
                TempData["Message"] = "User not found";
                TempData["MsgType"] = "warning";
                return RedirectToAction("Edit");
            }
        }

        [HttpPost]
        public IActionResult Edit(Users user)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Invalid Input";
                ViewData["MsgType"] = "warning";
                return View("Edit");
            }
            else
            {
                string update =
                   @"UPDATE Users
                    SET Group_id ='{1}', full_name='{2}', 
                        dob='{3:dd-MM-yyyy}'
                  WHERE full_name='{0}'";
                int res = DBUtl.ExecSQL(update, user.User_id ,  user.Group_id , user.full_name , user.dob);
                if (res == 1)
                {
                    TempData["Message"] = "User Updated";
                    TempData["MsgType"] = "success";
                }
                else
                {
                    TempData["Message"] = DBUtl.DB_Message;
                    TempData["MsgType"] = "danger";
                }
                return RedirectToAction("Index");
            }
        }
        public IActionResult Delete(int id)
        {
            string select = @"SELECT * FROM Users WHERE  User_id ={0}";
            DataTable ds = DBUtl.GetTable(select, id);
            if (ds.Rows.Count != 1)
            {
                TempData["Message"] = "User does not exist";
                TempData["MsgType"] = "warning";
            }
            else
            {
                string delete = "DELETE FROM Users WHERE User_id={0}";
                int res = DBUtl.ExecSQL(delete, id);
                if (res == 1)
                {
                    TempData["Message"] = "User Deleted";
                    TempData["MsgType"] = "success";
                }
                else
                {
                    TempData["Message"] = DBUtl.DB_Message;
                    TempData["MsgType"] = "danger";
                }
            }
            return RedirectToAction("Index");
        }
    }
}