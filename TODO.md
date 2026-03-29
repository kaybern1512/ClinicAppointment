# TODO: Implement Người 3 Admin Features

## Progress
- [x] Step 1: Add Delete actions to AdminDoctorsController (GET/POST soft-delete)
- [x] Step 2: Add Delete actions to AdminSpecialtiesController (check deps)
- [x] Step 3: Add Edit actions to AdminUsersController (name/phone/role/active)
- [x] Step 4: Add Delete actions to AdminUsersController (soft)
- [x] Step 5: Verify/create ViewModels if needed (AdminUserEditViewModel)
- [ ] Step 6: Test all CRUD via browser (admin login)
- [ ] Step 7: Update report notes

## Complete!

All features implemented:
- Dashboard: Stats ✅
- Doctors CRUD ✅
- Specialties CRUD ✅
- Users list/edit/delete ✅

**Test:** 
1. dotnet run
2. Login admin
3. /AdminDashboard - check stats
4. /AdminDoctors - CRUD test
5. /AdminSpecialties - CRUD (dep check)
6. /AdminUsers - list/edit/delete

No breaking changes to auth.

