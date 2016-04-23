using KAssets.Areas.Admin.Models;
using KAssets.Controllers;
using KAssets.Filters;
using KAssets.Models;
using KAssets.Resources;
using KAssets.Resources.Translation.AdminArea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KAssets.Areas.Admin.Controllers
{
    [RightCheck(Right = "Low admin")]
    public class SecurityGroupController : BaseController
    {
        //GET: Get all security group
        [HttpGet]
        public ActionResult GetAll()
        {
            var roles = this.securityGroupService.GetAll().ToList().ConvertAll(
                x =>
                new SecurityGroupViewModel
                {
                    Id = x.Id,
                    Name = x.Name
                });

            return View(roles);
        }

        //GET: Add security group
        [HttpGet]
        public ActionResult Add()
        {
            var rights = this.rightService.GetAll().ToList().
                ConvertAll(x => new SelectedRightViewModel
                {
                    Right = new RightViewModel
                    {
                        Name = Rights.ResourceManager.GetString("r" + x.Code.ToString()),
                        Id = x.Id
                    },
                    IsSelected = false
                });

            var viewModel = new AddSecurityGroupViewModel();
            viewModel.SelectedRights = rights;

            return View(viewModel);
        }

        //POST: Add security group
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Add(AddSecurityGroupViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Check security group exist
            if (this.securityGroupService.IsSecurityGroupExist(model.Name))
            {
                this.ModelState.AddModelError("", Resources.Translation.AdminArea.Admin.ThereAreSecGroup);
                return View(model);
            }

            //Check if the name is same of the base group
            if (model.Name == BaseAdmin.BaseGroup)
            {
                ModelState.AddModelError("", Resources.Translation.AdminArea.Admin.YouCantAddSameName);
                return View(model);
            }

            //Add a security group to database
            var scGroup = this.securityGroupService.Add(
                new SecurityGroup
                {
                    Name = model.Name
                });

            //Add right to a new security group
            var idS = model.SelectedRights.Where(x => x.IsSelected).Select(x => x.Right.Id).ToList();
            this.securityGroupService.AddRights(scGroup.Id, idS);

            return RedirectToAction("GetAll");
        }

        //POST: Remove security group
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Remove(int id)
        {
            if (id != 0)
            {
                if (this.securityGroupService.GetById(id).Name == BaseAdmin.BaseGroup)
                {
                    return RedirectToAction("GetAll");
                }
                this.securityGroupService.Remove(id);
            }

            return RedirectToAction("GetAll");
        }

        //GET: Edit security group
        [HttpGet]
        public ActionResult Edit(int id)
        {
            //If the id is of the base security gruop redirect to another page (security)
            if (this.securityGroupService.GetById(id).Name == BaseAdmin.BaseGroup)
            {
                return RedirectToAction("GetAll");
            }

            var role = this.securityGroupService.GetById(id);

            //Get selected rights
            var rights = this.rightService.GetAll().ToList().
                ConvertAll(x => new SelectedRightViewModel
                {
                    Right = new RightViewModel
                    {
                        Name = Rights.ResourceManager.GetString("r" + x.Code.ToString()),
                        Id = x.Id
                    },
                    IsSelected = role.Rights.Any(y=>y.Id==x.Id)?true:false
                });

            //Create viewmodel
            var viewModel = new EditSecurityGroupViewModel
            {
                Id = role.Id,
                Name = role.Name
            };

            viewModel.SelectedRights = rights;

            return View(viewModel);
        }

        //POST: Edit security group
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Edit(EditSecurityGroupViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Check if exist a security group with the same name
            if (this.securityGroupService.IsSecurityGroupExist(model.Name)&&this.securityGroupService.GetById(model.Id).Name!=model.Name)
            {
                this.ModelState.AddModelError("",  Resources.Translation.AdminArea.Admin.ThereAreSecGroup);
                return View(model);
            }
            
            //If the name is same as the name of base group
            if (model.Name ==BaseAdmin.BaseGroup)
            {
                return RedirectToAction("GetAll");
            }

            //Update the data
            this.securityGroupService.Update(
                new SecurityGroup
                {
                    Id = model.Id,
                    Name = model.Name,
                    Rights=model.SelectedRights.Where(x=>x.IsSelected).ToList()
                    .ConvertAll(x=>
                    new Right
                    {
                        Id=x.Right.Id,
                        Name=x.Right.Name
                    })
                });

            return RedirectToAction("GetAll");
        }

        //GET: Get details for certain security group
        [HttpGet]
        public ActionResult Details(int id)
        {
            var secGroup = this.securityGroupService.GetById(id);

            var viewModel = new DetailsSecurityGroupViewModel
            {
                Id = secGroup.Id,
                Name = secGroup.Name,
                Rights = secGroup.Rights.ToList().ConvertAll(
                x => new RightViewModel
                {
                    Id = x.Id,
                    Name = Rights.ResourceManager.GetString("r" + x.Code.ToString()),
                })
            };

            return View(viewModel);
        }
    }
}