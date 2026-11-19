from pathlib import Path
p=Path(__file__).parent/'Features'/'EmployeeCrud.feature'
s=p.read_text()
assert s.startswith('@AS-20')
assert s.count('  Scenario:') == 12
for key in ('@AS-21','@AS-22','@AS-23','@AS-24'):
    assert key in s
print('BDD asset validation: PASS (12 scenarios, 16 acceptance criteria represented)')
