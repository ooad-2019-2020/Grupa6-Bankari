﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EBANK.Data;
using EBANK.Models;
using EBANK.Models.ZahtjevZaKreditRepository;
using EBANK.Models.KreditRepository;
using EBANK.Models.BankarRepository;
using EBANK.Utils;

namespace EBANK.Controllers
{
    public class BankarZahtjevZaKreditController : Controller
    {
        private ZahtjeviZaKreditProxy _zahtjevi;
        private OOADContext Context;
        private Korisnik korisnik;

        public BankarZahtjevZaKreditController(OOADContext context)
        {
            _zahtjevi = new ZahtjeviZaKreditProxy(context);
            Context = context;
        }

        // GET: BankarZahtjevZaKredit
        public async Task<IActionResult> Index()
        {
            korisnik = await LoginUtils.Authenticate(Request, Context, this);
            if (korisnik == null) return RedirectToAction("Logout", "Login", new { area = "" });

            _zahtjevi.Pristupi(korisnik);
            return View(await _zahtjevi.DajSveZahtjeve());
        }

        // GET: BankarZahtjevZaKredit/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            korisnik = await LoginUtils.Authenticate(Request, Context, this);
            if (korisnik == null) return RedirectToAction("Logout", "Login", new { area = "" });

            _zahtjevi.Pristupi(korisnik);

            if (id == null)
            {
                return NotFound();
            }

            var zahtjev = await _zahtjevi.DajZahtjev(id);
            if (zahtjev == null)
            {
                return NotFound();
            }

            return View(zahtjev);
        }

        
        // POST: BakarZahtjevZaKredit/Odobri/5
        public async Task<IActionResult> Odobri(int id)
        {
            korisnik = await LoginUtils.Authenticate(Request, Context, this);
            if (korisnik == null) return RedirectToAction("Logout", "Login", new { area = "" });

            _zahtjevi.Pristupi(korisnik);

            ZahtjevZaKredit zahtjevZaKredit = await _zahtjevi.DajZahtjev(id);
            await _zahtjevi.RijesiZahtjev(id, true);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Odbij(int id)
        {
            korisnik = await LoginUtils.Authenticate(Request, Context, this);
            if (korisnik == null) return RedirectToAction("Logout", "Login", new { area = "" });

            _zahtjevi.Pristupi(korisnik);

            await _zahtjevi.RijesiZahtjev(id, false);
            return RedirectToAction(nameof(Index));
        }

        private bool ZahtjevZaKreditExists(int id)
        {
            return _zahtjevi.DaLiPostojiZahtjev(id);
        }
    }
}
